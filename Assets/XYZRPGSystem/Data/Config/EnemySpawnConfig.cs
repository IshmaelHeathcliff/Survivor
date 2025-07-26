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

        [Header("生成配置")]
        [ShowInInspector] public float GenerateStartTime { get; set; } = 0f;
        [ShowInInspector] public int GenerateStartWave { get; set; } = 1;
        [ShowInInspector] public float GenerateGap { get; set; } = 2f; // 生成间隔（秒）
        [ShowInInspector] public int GenerateCount { get; set; } = 1; // 每次生成数量
    }

    public class EnemySpawnGroupConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [ShowInInspector] public int TotalMaxCount { get; set; } = 100;
        [ShowInInspector] public float MinDistance { get; set; } = 5f;
        [ShowInInspector] public float MaxDistance { get; set; } = 10f;

        [ShowInInspector]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
        public List<EnemySpawnEntry> EnemyEntries { get; set; } = new();
    }
}
