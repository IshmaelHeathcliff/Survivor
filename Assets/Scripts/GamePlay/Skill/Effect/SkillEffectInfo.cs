using System;
using Sirenix.OdinInspector;

public abstract class SkillEffectInfo
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public string Description { get; set; }
}

public class AttackEffectInfo : SkillEffectInfo
{
    [ShowInInspector] public int Damage { get; set; }
    [ShowInInspector] public string AttackerAddress { get; set; }

    public AttackEffectInfo()
    {
        ID = "attack";
        Name = "攻击";
        Description = "施加攻击";
    }

}

public class ModifierEffectInfo : SkillEffectInfo
{
    [ShowInInspector] public string ModifierID { get; set; }
    [ShowInInspector] public int Value { get; set; }

    public ModifierEffectInfo()
    {
        ID = "modifier";
        Name = "修饰器";
        Description = "添加词条";
    }
}

public class SystemEffectInfo : SkillEffectInfo
{
    public SystemEffectInfo()
    {
        ID = "system";
        Name = "系统";
        Description = "系统联动";
    }
}
