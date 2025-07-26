using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace XYZRPGSystem.Data.Config
{
    public class EnemyConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string PrefabAddress { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [ShowInInspector] public float Health { get; set; } = 100f;
        [ShowInInspector] public float HealthIncreasePerWave { get; set; } = 10f;
        [ShowInInspector] public float MoveSpeed { get; set; } = 3f;
        [ShowInInspector] public float Damage { get; set; } = 1f;
        [ShowInInspector] public float DamageIncreasePerWave { get; set; } = 1f;
        [ShowInInspector] public float AttackRange { get; set; } = 1.5f;
        [ShowInInspector] public float AttackCooldown { get; set; } = 1f;
        [ShowInInspector] public float AttackSpeed { get; set; } = 1f;

        [ShowInInspector] public int CoinOnKill { get; set; } = 1;
        [ShowInInspector] public int WoodOnKill { get; set; } = 0;

        [ShowInInspector] public List<SkillInPool> SkillPool { get; set; } = new();
    }
}
