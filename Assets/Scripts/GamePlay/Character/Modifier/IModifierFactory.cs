using Data.Config;
using GamePlay.Character.Stat;

namespace GamePlay.Character.Modifier
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
