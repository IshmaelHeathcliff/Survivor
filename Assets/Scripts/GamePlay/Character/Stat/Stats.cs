using System.Collections.Generic;
using Data.Config;
using GamePlay.Character.Modifier;
using UnityEngine;

namespace GamePlay.Character.Stat
{
    public class Stats : IStatModifierFactory
    {
        protected Dictionary<string, IStat> InternalStats = new();

        public string FactoryID { get; set; }

        public void AddStat(IStat stat)
        {
            InternalStats[stat.Name] = stat;
        }


        public IEnumerable<IStat> GetAllStats()
        {
            return InternalStats.Values;
        }

        public IStat GetStat(string statName)
        {
            if (InternalStats.TryGetValue(statName, out IStat stat))
            {
                return stat;
            }
            else
            {
                Debug.LogError("未找到Stat: " + statName);
                return null;
            }
        }

        public IStat GetStat(StatModifierConfig modifierConfig)
        {
            return GetStat(modifierConfig.StatName);
        }

        #region StatModifier

        public IModifier CreateModifier(ModifierConfig modifierConfig)
        {
            if (modifierConfig is not StatModifierConfig config)
            {
                Debug.LogError($"modifierConfig {modifierConfig.ModifierID} is not a stat modifier");
                return null;
            }

            return CreateModifier(config);
        }

        public IStatModifier CreateModifier(StatModifierConfig modifierConfig, int value)
        {
            IStat stat = GetStat(modifierConfig.StatName);
            if (stat != null)
            {
                var modifier = new StatSingleFloatModifier(modifierConfig, stat, value)
                {
                    FactoryID = FactoryID
                };
                return modifier;
            }
            else
            {
                Debug.LogError("modifier stat name is not valid: " + modifierConfig.StatName);
                return null;
            }
        }

        public IStatModifier CreateModifier(StatModifierConfig modifierConfig)
        {
            IStat stat = GetStat(modifierConfig.StatName);
            if (stat != null)
            {
                var modifier = new StatSingleFloatModifier(modifierConfig, stat)
                {
                    FactoryID = FactoryID
                };
                if (modifier != null)
                {
                    modifier.RandomizeLevel();
                    modifier.RandomizeValue();
                }

                return modifier;
            }
            else
            {
                Debug.LogError("modifier stat name is not valid: " + modifierConfig.StatName);
                return null;
            }
        }

        #endregion
    }
}
