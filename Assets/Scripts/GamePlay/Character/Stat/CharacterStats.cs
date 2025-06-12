namespace Character.Stat
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

            { "Damage", new Stat("Damage") },
            { "CriticalChance", new Stat("CriticalChance") },
            { "CriticalMultiplier", new Stat("CriticalMultiplier") },
            { "Duration", new Stat("Duration")},
            { "CooldownInverse", new Stat("CooldownInverse")},
            { "AttackSpeed", new Stat("AttackSpeed") },
            { "AttackArea", new Stat("AttackArea") },
            { "ProjectileSpeed", new Stat("ProjectileSpeed") },
            { "ProjectileCount", new Stat("ProjectileCount") },
        };
        }
    }
}
