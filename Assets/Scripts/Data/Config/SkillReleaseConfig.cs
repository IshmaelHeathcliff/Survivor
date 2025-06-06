using System.Collections.Generic;
using Sirenix.OdinInspector;

public class SkillReleaseRuleConfig
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public string Description { get; set; }
    [ShowInInspector] public bool IsOnce { get; set; } = true;
    [ShowInInspector] public SkillReleaseConditionConfig Condition { get; set; }
    [ShowInInspector] public SkillReleaseRewardConfig Reward { get; set; }
}

public class SkillReleaseConditionConfig
{
    [ShowInInspector] public string Description { get; set; }
}

public class SkillReleaseRewardConfig
{
    [ShowInInspector] public string Description { get; set; }
}

public class SpecificSkillsReleaseConditionConfig : SkillReleaseConditionConfig
{
    [ShowInInspector] public List<string> RequiredSkillIDs { get; set; } = new();
    [ShowInInspector] public List<string> SkillsToRelease { get; set; } = new();
}

public class AnySkillsCountReleaseConditionConfig : SpecificSkillsReleaseConditionConfig
{
    [ShowInInspector] public int RequiredCount { get; set; }
}

public class SpecificSkillsReleaseRewardConfig : SkillReleaseRewardConfig
{
    [ShowInInspector] public List<string> NewSkillIDs { get; set; } = new();
}