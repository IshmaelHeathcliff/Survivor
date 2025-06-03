using System.Collections.Generic;
using Sirenix.OdinInspector;

public class SkillInfo
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public List<string> SkillEffectIDsOnUpdate { get; set; }
    [ShowInInspector] public List<string> SkillEffectIDsOnEnable { get; set; }
}

public class ActiveSkillInfo : SkillInfo
{
    [ShowInInspector] public float Cooldown { get; set; }

}

public class PassiveSkillInfo : SkillInfo
{

}