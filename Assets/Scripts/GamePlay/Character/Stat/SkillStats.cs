using System;
using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using Character.Stat;

public class SkillStats : Stats
{
    public SkillStats(IEnumerable<string> keywords, CharacterStats characterStats)
    {
        InternalStats = new()
        {
            { "Damage", new LocalStat(new Stat("Damage"), characterStats.GetStat("Damage")) },
            { "CriticalChance", new LocalStat(new Stat("CriticalChance"), characterStats.GetStat("CriticalChance")) },
            { "CriticalMultiplier", new LocalStat(new Stat("CriticalMultiplier"), characterStats.GetStat("CriticalMultiplier")) },
            { "Duration", new LocalStat(new Stat("Duration"), characterStats.GetStat("Duration"))},
            { "CooldownInverse", new LocalStat(new Stat("CooldownInverse"), characterStats.GetStat("CooldownInverse"))},
            { "AttackSpeed", new LocalStat(new Stat("AttackSpeed"), characterStats.GetStat("AttackSpeed")) },
            { "AttackArea", new LocalStat(new Stat("AttackArea"), characterStats.GetStat("AttackArea")) },
            { "ProjectileSpeed", new LocalStat(new Stat("ProjectileSpeed"), characterStats.GetStat("ProjectileSpeed")) },
            { "ProjectileCount", new LocalStat(new Stat("ProjectileCount"), characterStats.GetStat("ProjectileCount")) },
        };
    }
}


