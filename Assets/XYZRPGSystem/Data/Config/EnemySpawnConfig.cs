using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace XYZRPGSystem.Data.Config
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        [ShowInInspector] public string EnemyID { get; set; }
        [ShowInInspector] public int Weight { get; set; } = 1;
        [ShowInInspector] public int MinCount { get; set; } = 0;
        [ShowInInspector] public int MaxCount { get; set; } = 5;
        [ShowInInspector] public float SpawnChance { get; set; } = 1.0f;
        [ShowInInspector] public List<string> RequiredConditions { get; set; } = new();
    }

    public class EnemySpawnGroupConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [ShowInInspector] public int TotalMaxCount { get; set; } = 20;
        [ShowInInspector] public float GenerateGap { get; set; } = 2.0f;
        [ShowInInspector] public int GenerateCount { get; set; } = 1;
        [ShowInInspector] public float MinDistance { get; set; } = 5.0f;
        [ShowInInspector] public float MaxDistance { get; set; } = 10.0f;

        [ShowInInspector]
        [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
        public List<EnemySpawnEntry> EnemyEntries { get; set; } = new();

        /// <summary>
        /// 根据权重随机选择一个敌人配置ID
        /// </summary>
        public string GetRandomEnemyID()
        {
            if (EnemyEntries == null || EnemyEntries.Count == 0)
                return null;

            // 过滤有效的敌人条目
            var validEntries = EnemyEntries.Where(entry =>
                !string.IsNullOrEmpty(entry.EnemyID) &&
                entry.Weight > 0 &&
                UnityEngine.Random.value <= entry.SpawnChance).ToList();

            if (validEntries.Count == 0)
                return null;

            // 计算总权重
            int totalWeight = validEntries.Sum(entry => entry.Weight);
            if (totalWeight <= 0)
                return validEntries[0].EnemyID;

            // 权重随机选择
            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var entry in validEntries)
            {
                currentWeight += entry.Weight;
                if (randomValue < currentWeight)
                {
                    return entry.EnemyID;
                }
            }

            return validEntries.Last().EnemyID;
        }

        /// <summary>
        /// 获取指定敌人类型的当前最大数量
        /// </summary>
        public int GetMaxCountForEnemy(string enemyID)
        {
            var entry = EnemyEntries?.FirstOrDefault(e => e.EnemyID == enemyID);
            return entry?.MaxCount ?? 0;
        }

        /// <summary>
        /// 获取指定敌人类型的最小数量
        /// </summary>
        public int GetMinCountForEnemy(string enemyID)
        {
            var entry = EnemyEntries?.FirstOrDefault(e => e.EnemyID == enemyID);
            return entry?.MinCount ?? 0;
        }
    }
}
