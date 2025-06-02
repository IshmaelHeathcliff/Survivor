using System;
using Character.Stat;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Character.Modifier
{
    public interface IStatModifier : IModifier<StatModifierInfo>
    {
        IStat GetStat();
        void RandomizeLevel();
        void RandomizeValue();
    }

    public interface IStatModifier<T> : IStatModifier
    {
        T Value { get; set; }
    }

    public interface IStatModifier<T1, T2> : IStatModifier<T1>
    {
        T2 Value2 { get; set; }
    }

    public abstract class StatModifier : Modifier<StatModifierInfo>, IStatModifier
    {
        protected static IStat GetStat(IStatModifier modifier)
        {
            return GameFrame.Interface.GetSystem<ModifierSystem>().GetStat(modifier);
        }

        public int Level { get; set; }

        protected IStat Stat;

        [JsonConstructor]
        protected StatModifier()
        {
        }

        protected StatModifier(StatModifierInfo modifierInfo, IStat stat)
        {
            ModifierInfo = modifierInfo;
            Stat = stat;
            ModifierID = modifierInfo.ModifierID;
        }

        public abstract void RandomizeLevel();
        public abstract void RandomizeValue();

        public IStat GetStat()
        {
            return Stat;
        }
    }


    [Serializable]
    public abstract class StatModifier<T> : StatModifier, IStatModifier<T>
    {
        public T Value { get; set; }


        [JsonConstructor]
        protected StatModifier()
        {
        }

        protected StatModifier(StatModifierInfo modifierInfo, IStat stat) : base(modifierInfo, stat)
        {
        }
    }

    [Serializable]
    public abstract class StatModifier<T1, T2> : StatModifier<T1>, IStatModifier<T1, T2>
    {
        public T2 Value2 { get; set; }

        [JsonConstructor]
        protected StatModifier()
        {
        }

        protected StatModifier(StatModifierInfo modifierInfo, IStat stat) : base(modifierInfo, stat)
        {
        }
    }

    [Serializable]
    public struct LevelRange
    {
        [ShowInInspector] public int Min { get; set; }
        [ShowInInspector] public int Max { get; set; }
    }

    public enum StatModifierType
    {
        Base,
        Added,
        Increase,
        More,
        Fixed,
    }


    [Serializable]
    public class StatModifierInfo : ModifierInfo
    {
        [ShowInInspector] public string StatName { get; set; }
        [ShowInInspector] public StatModifierType StatModifierType { get; set; }
        [ShowInInspector] public int MaxLevel { get; set; }
        [ShowInInspector][TableList(ShowIndexLabels = true)] public LevelRange[] LevelRanges { get; set; }
    }


}
