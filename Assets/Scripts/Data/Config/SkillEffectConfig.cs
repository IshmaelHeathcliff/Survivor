using System;
using Sirenix.OdinInspector;

public abstract class SkillEffectConfig
{
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public string Description { get; set; }
}

public class AttackEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public int Damage { get; set; }
    [ShowInInspector] public string AttackerAddress { get; set; }

    public AttackEffectConfig()
    {
        ID = "attack";
        Name = "攻击";
        Description = "施加攻击";
    }

}

public class ModifierEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public string ModifierID { get; set; }
    [ShowInInspector] public int Value { get; set; }

    public ModifierEffectConfig()
    {
        ID = "modifier";
        Name = "修饰器";
        Description = "添加词条";
    }
}

public class SystemEffectConfig : SkillEffectConfig
{
    public SystemEffectConfig()
    {
        ID = "system";
        Name = "系统";
        Description = "系统联动";
    }
}
