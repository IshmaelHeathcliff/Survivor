using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Data.Config
{
    public struct SkillInPool
    {
        [ShowInInspector] public string Name { get; set; }
    }

    public abstract class SkillPoolAddRuleConfig
    {
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public List<string> SkillIDsToAdd { get; set; }
    }

    public class SpecificSkillsPoolAddRuleConfig : SkillPoolAddRuleConfig
    {
        [ShowInInspector] public List<string> RequiredSkillIDs { get; set; }
    }
}
