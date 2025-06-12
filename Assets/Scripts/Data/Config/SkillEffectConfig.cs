using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public abstract class SkillEffectConfig
{
    [ShowInInspector][ReadOnly] public string Description { get; set; }
}


public class NestedEffectConfig : SkillEffectConfig
{
    [ShowInInspector, PropertyOrder(int.MaxValue)] public List<SkillEffectConfig> ChildEffects { get; set; }
}

public class AttackEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public string AttackerID { get; set; }

    public AttackEffectConfig()
    {
        Description = "创建攻击器";
    }
}

public class AcquireSkillEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public string SkillID { get; set; }

    public AcquireSkillEffectConfig()
    {
        Description = "获取技能";
    }
}

public class ModifierEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public string ModifierID { get; set; }
    [ShowInInspector] public int Value { get; set; }

    public ModifierEffectConfig()
    {
        Description = "添加词条";
    }
}

public class LocalModifierEffectConfig : ModifierEffectConfig
{
    [ShowInInspector] public string AttackSkillID { get; set; }
    public LocalModifierEffectConfig()
    {
        Description = "添加局部词条";
    }
}

public class CountIncrementEffectConfig : NestedEffectConfig
{
    [ShowInInspector] public string CountValueID { get; set; }
    [ShowInInspector] public int Increment { get; set; }

    public CountIncrementEffectConfig()
    {
        Description = "增量触发";
    }
}

public class RollDiceEffectConfig : NestedEffectConfig
{
    public RollDiceEffectConfig()
    {
        Description = "掷骰子";
    }
}

public class DiceOnValueEffectConfig : NestedEffectConfig
{
    [ShowInInspector] public int Value { get; set; }

    public DiceOnValueEffectConfig()
    {
        Description = "骰子数触发";
    }
}

public class AcquireResourceEffectConfig : SkillEffectConfig
{
    [ShowInInspector] public string ResourceID { get; set; }
    [ShowInInspector] public int Amount { get; set; }

    public AcquireResourceEffectConfig()
    {
        Description = "获取资源";
    }
}
