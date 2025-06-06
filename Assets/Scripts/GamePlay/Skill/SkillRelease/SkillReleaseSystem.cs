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
            if (rule.Condition is ReleaseOnSkillAcquiredCondition skillReleaseCondition)
            {
                this.RegisterEvent<SkillAcquiredEvent>(skillReleaseCondition.CheckCondition);
            }
        }
    }

    void RegisterRewards()
    {
        foreach (SkillReleaseRule rule in _releaseRules.Values)
        {
            rule.Condition.OnCondition.Register(() =>
                {
                    rule.HasTriggered = true;
                    _releaseRules.Remove(rule.ID);
                    _triggeredRules.Add(rule.ID);
                });

            if (rule.Reward is SpecificSkillsReleaseReward skillReleaseReward)
            {
                rule.Condition.OnCondition.Register(() =>
                {
                    foreach (string skillID in skillReleaseReward.NewSkillIDs)
                    {
                        _skillSystem.AcquireSkill(skillID);
                    }
                });
            }


        }
    }
}
