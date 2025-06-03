﻿using System;
using Character.Stat;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Character.Modifier
{
    [Serializable]
    public class StatSingleIntModifier : StatModifier<int>
    {
        [JsonConstructor]
        public StatSingleIntModifier()
        {

        }

        public StatSingleIntModifier(StatModifierInfo modifierInfo, IStat stat) : base(modifierInfo, stat)
        {
        }

        public StatSingleIntModifier(StatModifierInfo modifierInfo, IStat stat, int value) : base(modifierInfo,
            stat)
        {
            Level = 1;
            Value = value;
        }

        public override string GetDescription()
        {
            return Value >= 0 ?
                string.Format(ModifierInfo.PositiveDescription, Stat.Name, Value) :
                string.Format(ModifierInfo.NegativeDescription, Stat.Name, -Value);
        }

        public override void Check()
        {
            // TODO: 检查Modifier生效条件
            throw new System.NotImplementedException();
        }

        public override void Register()
        {
            switch (ModifierInfo.StatModifierType)
            {
                case StatModifierType.Added:
                    Stat.AddAddedValueModifier(InstanceID, this);
                    break;
                case StatModifierType.Increase:
                    Stat.AddIncreaseModifier(InstanceID, this);
                    break;
                case StatModifierType.More:
                    Stat.AddMoreModifier(InstanceID, this);
                    break;
                case StatModifierType.Fixed:
                    Stat.AddFixedValueModifier(InstanceID, this);
                    break;
                default:
                    break;
            }

        }

        public override void Unregister()
        {
            switch (ModifierInfo.StatModifierType)
            {
                case StatModifierType.Added:
                    Stat.RemoveAddedValueModifier(InstanceID);
                    break;
                case StatModifierType.Increase:
                    Stat.RemoveIncreaseModifier(InstanceID);
                    break;
                case StatModifierType.More:
                    Stat.RemoveMoreModifier(InstanceID);
                    break;
                case StatModifierType.Fixed:
                    Stat.RemoveFixedValueModifier(InstanceID);
                    break;
                default:
                    break;
            }

        }

        public override void Load()
        {
            //TODO: 不使用全局静态调用？
            ModifierInfo = GetModifierInfo(ModifierID) as StatModifierInfo;
            Stat = GetStat(this);

        }

        public override void RandomizeLevel()
        {
            if (ModifierInfo is StatModifierInfo info)
            {
                Level = Random.Range(0, info.MaxLevel);
            }

        }

        public override void RandomizeValue()
        {
            if (ModifierInfo is StatModifierInfo info)
            {
                LevelRange levelRange = info.LevelRanges[Level];
                Value = Random.Range(levelRange.Min, levelRange.Max + 1);
            }

        }
    }

    [Serializable]
    public class StatDoubleIntModifier : StatSingleIntModifier, IStatModifier<int, int>
    {
        public int Value2 { get; set; }
    }
}
