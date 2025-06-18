using System.Collections.Generic;

namespace GamePlay.Character.Stat
{
    public class CharacterStats : Stats
    {
        public CharacterStats()
        {
            InternalStats = new()
        {
            { "Health", new ConsumableStat("Health") },
            { "HealthRegen", new Stat("HealthRegen") },
            { "MoveSpeed", new Stat("MoveSpeed") },

            { "CoinGain", new Stat("CoinGain") },
            { "WoodGain", new Stat("WoodGain") },

            { "Damage", new KeywordStat("Damage") },
            { "CriticalChance", new KeywordStat("CriticalChance") },
            { "CriticalMultiplier", new KeywordStat("CriticalMultiplier") },
            { "Duration", new KeywordStat("Duration")},
            { "CooldownInverse", new KeywordStat("CooldownInverse")},
            { "AttackSpeed", new KeywordStat("AttackSpeed") },
            { "AttackArea", new KeywordStat("AttackArea") },
            { "ProjectileSpeed", new KeywordStat("ProjectileSpeed") },
            { "ProjectileCount", new KeywordStat("ProjectileCount") },
            { "ChainCount", new KeywordStat("ChainCount") },
            { "PenetrateCount", new KeywordStat("PenetrateCount") },
            { "SplitCount", new KeywordStat("SplitCount") },
        };
        }
    }
}
