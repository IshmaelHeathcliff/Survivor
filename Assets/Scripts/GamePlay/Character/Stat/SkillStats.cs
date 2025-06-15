using System.Collections.Generic;

namespace GamePlay.Character.Stat
{
    public class SkillStats : Stats
    {
        public SkillStats(List<string> keywords, CharacterStats characterStats)
        {
            InternalStats = new()
            {
                { "Damage", new LocalKeywordStat(keywords, new KeywordStat("Damage"), characterStats.GetKeywordStat("Damage")) },
                { "CriticalChance", new LocalKeywordStat(keywords, new KeywordStat("CriticalChance"), characterStats.GetKeywordStat("CriticalChance")) },
                { "CriticalMultiplier", new LocalKeywordStat(keywords, new KeywordStat("CriticalMultiplier"), characterStats.GetKeywordStat("CriticalMultiplier")) },
                { "Duration", new LocalKeywordStat(keywords, new KeywordStat("Duration"), characterStats.GetKeywordStat("Duration"))},
                { "CooldownInverse", new LocalKeywordStat(keywords, new KeywordStat("CooldownInverse"), characterStats.GetKeywordStat("CooldownInverse"))},
                { "AttackSpeed", new LocalKeywordStat(keywords, new KeywordStat("AttackSpeed"), characterStats.GetKeywordStat("AttackSpeed")) },
                { "AttackArea", new LocalKeywordStat(keywords, new KeywordStat("AttackArea"), characterStats.GetKeywordStat("AttackArea")) },
                { "ProjectileSpeed", new LocalKeywordStat(keywords, new KeywordStat("ProjectileSpeed"), characterStats.GetKeywordStat("ProjectileSpeed")) },
                { "ProjectileCount", new LocalKeywordStat(keywords, new KeywordStat("ProjectileCount"), characterStats.GetKeywordStat("ProjectileCount")) },
            };
        }
    }
}


