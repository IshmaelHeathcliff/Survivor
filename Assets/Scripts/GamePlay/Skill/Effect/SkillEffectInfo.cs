using System;
using Sirenix.OdinInspector;

public class SkillEffectInfo
{
    [ShowInInspector] public string EffectType { get; set; }
    [ShowInInspector] public string ID { get; set; }
    [ShowInInspector] public string Name { get; set; }
    [ShowInInspector] public string Description { get; set; }
}

public class AttackEffectInfo : SkillEffectInfo
{
    [ShowInInspector] public int Damage { get; set; }
    [ShowInInspector] public string AttackerAddress { get; set; }
}

public class ModifierEffectInfo : SkillEffectInfo
{
    [ShowInInspector] public string ModifierID { get; set; }
    [ShowInInspector] public int Value { get; set; }
}