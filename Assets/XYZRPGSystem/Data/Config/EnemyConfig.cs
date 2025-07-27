using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XYZRPGSystem.Data.Config
{
    public class EnemyConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string PrefabAddress { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [Header("CharacterStats")]
        [ShowInInspector] public float Health { get; set; } = 100f;
        [ShowInInspector] public float HealthRegen { get; set; } = 0f;
        [ShowInInspector] public float MoveSpeed { get; set; } = 3f;

        [Header("Skill Stats")]
        [ShowInInspector] public float Damage { get; set; } = 1f;
        [ShowInInspector] public float AttackSpeed { get; set; } = 4f;
        [ShowInInspector] public float AttackArea { get; set; } = 1f;
        [ShowInInspector] public float AttackRange { get; set; } = 4f;
        [ShowInInspector] public float CooldownInverse { get; set; } = 1f;

        [Header("Enemy Stats")]
        [ShowInInspector] public int CoinOnDead { get; set; } = 1;
        [ShowInInspector] public int WoodOnDead { get; set; } = 0;
        [ShowInInspector] public float HealthIncreasePerWave { get; set; } = 10f;
        [ShowInInspector] public float DamageIncreasePerWave { get; set; } = 1f;


        [ShowInInspector] public List<string> SkillIDs { get; set; } = new();
    }
}
