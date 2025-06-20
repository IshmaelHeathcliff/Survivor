using System.Collections.Generic;
using Data.Config;
using GamePlay.Character;
using Data.SaveLoad;

namespace GamePlay.Skill
{
    public class SkillReleaseSystem : AbstractSystem
    {
        readonly Dictionary<string, SkillReleaseRule> _releaseRules = new();
        readonly SkillReleaseConfigLoader _skillReleaseConfigLoader = new();

        const string JsonPath = "Preset";
        const string JsonName = "SkillReleaseRules.json";

        SkillSystem _skillSystem;

        // 保存注册的事件，用于在OnDeinit时取消注册
        readonly List<IUnRegister> _unRegisters = new();

        protected override void OnInit()
        {
            _skillSystem = this.GetSystem<SkillSystem>();

            LoadRules();
        }

        void LoadRules()
        {
            List<SkillReleaseRuleConfig> configs = this.GetUtility<SaveLoadUtility>().Load<List<SkillReleaseRuleConfig>>(JsonName, JsonPath);

            if (configs != null)
            {
                foreach (SkillReleaseRuleConfig config in configs)
                {
                    _releaseRules.Add(config.ID, _skillReleaseConfigLoader.CreateRule(config));
                }
            }
        }

        public void RegisterConditions(ICharacterModel model)
        {
            foreach (SkillReleaseRule rule in _releaseRules.Values)
            {
                switch (rule.Condition)
                {
                    case ReleaseOnSkillAcquiredCondition skillReleaseCondition:
                        _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(skillReleaseCondition.CheckCondition));
                        break;
                    case ValueCountCondition valueCountCondition:
                        _unRegisters.Add(this.GetSystem<CountSystem>().Register(valueCountCondition.ValueID, model, e => valueCountCondition.CheckCondition(e)));
                        break;
                    case CompositeAndReleaseCondition compositeAndReleaseCondition:
                        _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(compositeAndReleaseCondition.CheckCondition));
                        break;
                    case CompositeOrReleaseCondition compositeOrReleaseCondition:
                        _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(compositeOrReleaseCondition.CheckCondition));
                        break;
                }
            }
        }

        public void RegisterRewards(ICharacterModel model)
        {
            foreach (SkillReleaseRule rule in _releaseRules.Values)
            {
                _unRegisters.Add(rule.Condition.OnRelease.Register((e) =>
                    {
                        foreach (string skillID in rule.Condition.SkillsToRelease)
                        {
                            _skillSystem.ReleaseSkill(skillID, e.Model);
                        }
                        rule.HasTriggered = true;
                    }));

                if (rule.Reward is SpecificSkillsReleaseReward skillReleaseReward)
                {
                    _unRegisters.Add(rule.Condition.OnRelease.Register((e) =>
                    {
                        foreach (string skillID in skillReleaseReward.NewSkillIDs)
                        {
                            _skillSystem.AcquireSkill(skillID, e.Model);
                        }
                    }));
                }
            }
        }

        protected override void OnDeinit()
        {
            foreach (IUnRegister unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }

            _unRegisters.Clear();
        }
    }
}
