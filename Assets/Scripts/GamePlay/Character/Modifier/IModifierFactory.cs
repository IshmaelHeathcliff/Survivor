﻿using Character.Stat;

namespace Character.Modifier
{
    public interface IModifierFactory
    {
        string FactoryID { get; set; }
        IModifier CreateModifier(ModifierConfig modifierConfig);
    }

    public interface IStatModifierFactory : IModifierFactory
    {
        IStat GetStat(StatModifierConfig modifierConfig);
        IStatModifier CreateModifier(StatModifierConfig modifierConfig, int value);
        IStatModifier CreateModifier(StatModifierConfig modifierConfig);
    }
}
