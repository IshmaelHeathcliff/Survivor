using System;
using System.Collections.Generic;
using UnityEngine;
using XYZRPGSystem.Gameplay.Skill;
using Sirenix.OdinInspector;

namespace Data.Config
{
    public class LevelConfig
    {
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public string ID { get; set; }
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public string Name { get; set; }
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public string Description { get; set; }
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public int LevelNumber { get; set; }
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public string NextLevelID { get; set; }
        [BoxGroup("关卡基本信息")]
        [ShowInInspector] public string SceneName { get; set; }

        [BoxGroup("关卡波次配置")]
        [ListDrawerSettings(
            ShowIndexLabels = true,
            AddCopiesLastElement = true,
            DefaultExpandedState = true,
            ListElementLabelName = "WaveNumber"
            )]
        [ShowInInspector] public List<LevelWaveConfig> WaveConfigs { get; set; } = new List<LevelWaveConfig>();
    }

    public class LevelWaveConfig
    {
        [BoxGroup("基本信息")]
        [ShowInInspector] public int WaveNumber { get; set; }

        [BoxGroup("敌人生成配置")]
        [ShowInInspector] public string EnemySpawnGroupID { get; set; }
        [BoxGroup("敌人生成配置")]
        [ShowInInspector] public float EnemySpawnRateMultiplier { get; set; } = 1.0f;
        [BoxGroup("敌人生成配置")]
        [ShowInInspector] public float EnemyHealthMultiplier { get; set; } = 1.0f;
        [BoxGroup("敌人生成配置")]
        [ShowInInspector] public float EnemyDamageMultiplier { get; set; } = 1.0f;
        [BoxGroup("敌人生成配置")]
        [ShowInInspector] public int MaxEnemyCount { get; set; } = 100;

        [BoxGroup("技能抽取概率配置")]
        [ShowInInspector] public List<SkillRarityWeight> SkillRarityWeights { get; set; } = new List<SkillRarityWeight>();

        [BoxGroup("关卡奖励")]
        [ShowInInspector] public List<LevelReward> Rewards { get; set; } = new List<LevelReward>();

        [BoxGroup("关卡条件")]
        [ShowInInspector] public float Duration { get; set; } = 60f;
        [BoxGroup("关卡条件")]
        [ShowInInspector] public int KillTarget { get; set; } = 0;
    }

    public class SkillRarityWeight
    {
        [ShowInInspector] public SkillRarity Rarity { get; set; }
        [ShowInInspector] public int Weight { get; set; }

        public SkillRarityWeight() { }

        public SkillRarityWeight(SkillRarity rarity, int weight)
        {
            Rarity = rarity;
            Weight = weight;
        }
    }

    public class LevelReward
    {
        [ShowInInspector] public RewardType Type { get; set; }
        [ShowInInspector] public int Amount { get; set; }

        public enum RewardType
        {
            Coin,
            Wood,
        }
    }
}
