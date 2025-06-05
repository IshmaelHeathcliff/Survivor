using System.Collections.Generic;
using Sirenix.OdinInspector;

public class SkillConfig
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public List<SkillEffectConfig> SkillEffectConfigsOnEnable { get; set; }
}

public class ActiveSkillConfig : SkillConfig
{
    [ShowInInspector] public List<SkillEffectConfig> SkillEffectConfigsOnUpdate { get; set; }
    [ShowInInspector] public float Cooldown { get; set; }

}

public class PassiveSkillConfig : SkillConfig
{

}
