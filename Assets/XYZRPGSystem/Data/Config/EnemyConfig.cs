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

        [ShowInInspector] public int MaxCount { get; set; } = 10;
        [ShowInInspector] public int GenerateCount { get; set; } = 1;
        [ShowInInspector] public float GenerateGap { get; set; } = 2.0f;
        [ShowInInspector] public float MinDistance { get; set; } = 5.0f;
        [ShowInInspector] public float MaxDistance { get; set; } = 10.0f;

        [ShowInInspector] public float Health { get; set; } = 100.0f;
        [ShowInInspector] public float MoveSpeed { get; set; } = 3.0f;
        [ShowInInspector] public float AttackDamage { get; set; } = 10.0f;
        [ShowInInspector] public float AttackRange { get; set; } = 1.5f;
        [ShowInInspector] public float AttackCooldown { get; set; } = 1.0f;

        [ShowInInspector] public int CoinReward { get; set; } = 1;
        [ShowInInspector] public int WoodReward { get; set; } = 0;

        [ShowInInspector] public List<string> DropIDs { get; set; } = new();
        [ShowInInspector] public List<string> Keywords { get; set; } = new();
        [ShowInInspector] public string SkillPoolPath { get; set; } = "";
    }
}
