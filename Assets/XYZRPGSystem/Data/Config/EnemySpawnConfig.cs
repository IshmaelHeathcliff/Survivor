using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace XYZRPGSystem.Data.Config
{
    public class EnemySpawnEntry
    {
        [ShowInInspector] public string EnemyID { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [BoxGroup("生成配置")]
        [ShowInInspector] public float GenerateStartTime { get; set; } = 0f;
        [BoxGroup("生成配置")]
        [ShowInInspector] public int GenerateStartWave { get; set; } = 1;
        [BoxGroup("生成配置")]
        [ShowInInspector] public float GenerateGap { get; set; } = 2f; // 生成间隔（秒）
        [BoxGroup("生成配置")]
        [ShowInInspector] public int GenerateCount { get; set; } = 1; // 每次生成数量
        [BoxGroup("生成配置")]
        [ShowInInspector] public int MaxCount { get; set; } = 10; // 最大生成数量
    }

    public class EnemySpawnGroupConfig
    {
        [BoxGroup("基本信息")]
        [ShowInInspector] public string ID { get; set; }
        [BoxGroup("基本信息")]
        [ShowInInspector] public string Name { get; set; }
        [BoxGroup("基本信息")]
        [ShowInInspector] public string Description { get; set; }

        [BoxGroup("生成配置")]
        [ShowInInspector] public int TotalMaxCount { get; set; } = 100;
        [BoxGroup("生成配置")]
        [ShowInInspector] public float MinDistance { get; set; } = 5f;
        [BoxGroup("生成配置")]
        [ShowInInspector] public float MaxDistance { get; set; } = 10f;

        [BoxGroup("敌人配置")]
        [ShowInInspector]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
        public List<EnemySpawnEntry> EnemyEntries { get; set; } = new();
    }
}
