using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Character.Damage;
using Character.Player;
using UnityEngine;

public class SkillReleaseSystem : AbstractSystem
{
    readonly Dictionary<string, SkillReleaseRule> _releaseRules = new();

    const string JsonPath = "Preset";
    const string JsonName = "SkillReleaseRules.json";
    readonly List<string> _triggeredRules = new(); // 记录已触发的一次性规则

    SkillSystem _skillSystem;

    List<IUnRegister> _unRegisters = new();

    protected override void OnInit()
    {
        _skillSystem = this.GetSystem<SkillSystem>();

        LoadRules();
        RegisterConditions();
        RegisterRewards();
    }

    void LoadRules()
    {
        List<SkillReleaseRuleConfig> configs = this.GetUtility<SaveLoad.SaveLoadUtility>().Load<List<SkillReleaseRuleConfig>>(JsonName, JsonPath);

        if (configs != null)
        {
            foreach (SkillReleaseRuleConfig config in configs)
            {
                _releaseRules.Add(config.ID, SkillReleaseConfigLoader.CreateRule(config));
            }
        }
    }

    void RegisterConditions()
    {
        foreach (SkillReleaseRule rule in _releaseRules.Values)
        {
            switch (rule.Condition)
            {
                case ReleaseOnSkillAcquiredCondition skillReleaseCondition:
                    _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(skillReleaseCondition.CheckCondition));
                    break;
                case ValueCountCondition valueCountCondition:
                    _unRegisters.Add(this.GetSystem<CountSystem>().Register(valueCountCondition.ValueID, e => valueCountCondition.CheckCondition(e)));
                    break;
            }
        }
    }

    void RegisterRewards()
    {
        foreach (SkillReleaseRule rule in _releaseRules.Values)
        {
            _unRegisters.Add(rule.Condition.OnCondition.Register(() =>
                {
                    rule.HasTriggered = true;
                    _releaseRules.Remove(rule.ID);
                    _triggeredRules.Add(rule.ID);
                }));

            if (rule.Reward is SpecificSkillsReleaseReward skillReleaseReward)
            {
                _unRegisters.Add(rule.Condition.OnCondition.Register(() =>
                {
                    foreach (string skillID in skillReleaseReward.NewSkillIDs)
                    {
                        _skillSystem.AcquireSkill(skillID);
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
