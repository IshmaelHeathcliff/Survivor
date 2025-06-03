using System.Collections.Generic;
using Sirenix.OdinInspector;

public class SkillInfo
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public List<SkillEffectInfo> SkillEffectInfosOnUpdate { get; set; }
    [ShowInInspector] public List<SkillEffectInfo> SkillEffectInfosOnEnable { get; set; }
}

public class ActiveSkillInfo : SkillInfo
{
    [ShowInInspector] public float Cooldown { get; set; }

}

public class PassiveSkillInfo : SkillInfo
{

}