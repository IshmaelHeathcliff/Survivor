using System.Collections.Generic;
using Character.Modifier;
using UnityEngine;

namespace Character.Stat
{
    public class Stats : IStatModifierFactory
    {
        protected Dictionary<string, IStat> InternalStats = new()
        {
            { "Health", new ConsumableStat("Health") },
            { "HealthRegen", new Stat("HealthRegen") },
            { "MoveSpeed", new Stat("MoveSpeed") },

            { "CoinGain", new Stat("CoinGain") },
            { "WoodGain", new Stat("WoodGain") },
            { "CoinInterest", new Stat("CoinInterest") },
            { "WoodInterest", new Stat("WoodInterest") },

            { "Damage", new KeywordStat("Damage") },
            { "CriticalChance", new Stat("CriticalChance") },
            { "CriticalMultiplier", new Stat("CriticalMultiplier") },
            { "Duration", new Stat("Duration")},
            { "Cooldown", new Stat("Cooldown")},
            { "AttackSpeed", new Stat("AttackSpeed") },
            { "AttackArea", new Stat("AttackArea") },
        };

        public string FactoryID { get; set; }


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
                var modifier = new StatSingleIntModifier(modifierConfig, stat, value)
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
                var modifier = new StatSingleIntModifier(modifierConfig, stat)
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
