using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;


namespace Character.Modifier
{
    [Serializable]
    public abstract class ModifierConfig
    {
        [ShowInInspector] public string ModifierID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string PositiveDescription { get; set; }
        [ShowInInspector] public string NegativeDescription { get; set; }
        [ShowInInspector] public List<string> Keywords { get; set; }
    }

    [Serializable]
    public class StatModifierConfig : ModifierConfig
    {
        [ShowInInspector] public string StatName { get; set; }
        [ShowInInspector] public StatModifierType StatModifierType { get; set; }
        [ShowInInspector] public int MaxLevel { get; set; }
        [ShowInInspector][TableList(ShowIndexLabels = true)] public LevelRange[] LevelRanges { get; set; }
    }
}