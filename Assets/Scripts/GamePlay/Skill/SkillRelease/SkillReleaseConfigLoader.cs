using System;
using System.Collections.Generic;

public static class SkillReleaseConfigLoader
{
    public static SkillReleaseRule CreateRule(SkillReleaseRuleConfig config)
    {
        return new SkillReleaseRule(config.ID, SkillReleaseConditionConfigLoader.CreateCondition(config.Condition), SkillReleaseRewardConfigLoader.CreateReward(config.Reward), config.IsOnce);
    }
}

public static class SkillReleaseConditionConfigLoader
{
    public static ISkillReleaseCondition CreateCondition(SkillReleaseConditionConfig config)
    {
        switch (config)
        {
            case AnySkillsCountReleaseConditionConfig anySkillCountConfig:
                return new AnySkillsCountReleaseCondition(anySkillCountConfig.RequiredSkillIDs, anySkillCountConfig.SkillsToRelease, anySkillCountConfig.RequiredCount, anySkillCountConfig.Description);
            case SpecificSkillsReleaseConditionConfig specificSkillsConfig:
                return new SpecificSkillsReleaseCondition(specificSkillsConfig.RequiredSkillIDs, specificSkillsConfig.SkillsToRelease, specificSkillsConfig.Description);
            case ValueCountConditionConfig valueCountConfig:
                return new ValueCountCondition(valueCountConfig.ValueID, valueCountConfig.Value, valueCountConfig.SkillsToRelease, valueCountConfig.Description);
            default:
                throw new Exception($"Unknown skill release condition type: {config.GetType().Name}");
        }
    }
}

public static class SkillReleaseRewardConfigLoader
{
    public static ISkillReleaseReward CreateReward(SkillReleaseRewardConfig config)
    {
        switch (config)
        {
            case SpecificSkillsReleaseRewardConfig specificSkillsConfig:
                return new SpecificSkillsReleaseReward(specificSkillsConfig.NewSkillIDs, specificSkillsConfig.Description);
            default:
                throw new Exception($"Unknown skill release reward type: {config.GetType().Name}");
        }
    }
}
