using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XYZRPGSystem.Core;
using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Data.SaveLoad;

namespace XYZRPGSystem.Gameplay
{
    public class EnemySystem : AbstractSystem
    {
        #region Enemy Config Management
        const string EnemyConfigPath = "Preset/JSON";
        const string EnemyConfigName = "Enemies.json";

        bool _enemyConfigLoaded = false;
        readonly Dictionary<string, EnemyConfig> _enemyConfigs = new();

        void LoadEnemyConfigs()
        {
            if (_enemyConfigLoaded) return;

            _enemyConfigs.Clear();
            List<EnemyConfig> configs = SaveLoadManager.Load<List<EnemyConfig>>(EnemyConfigName, EnemyConfigPath);

            if (configs != null)
            {
                foreach (EnemyConfig config in configs)
                {
                    _enemyConfigs[config.ID] = config;
                }
                Debug.Log($"Loaded {_enemyConfigs.Count} enemy configs");
            }
            else
            {
                Debug.LogWarning($"Failed to load enemy configs from {EnemyConfigName}");
            }

            _enemyConfigLoaded = true;
        }

        public EnemyConfig GetEnemyConfig(string id)
        {
            LoadEnemyConfigs();

            if (_enemyConfigs.TryGetValue(id, out EnemyConfig config))
            {
                return config;
            }

            Debug.LogWarning($"Enemy config not found: {id}");
            return null;
        }

        public List<EnemyConfig> GetAllEnemyConfigs()
        {
            LoadEnemyConfigs();
            return _enemyConfigs.Values.ToList();
        }

        public bool HasEnemyConfig(string id)
        {
            LoadEnemyConfigs();
            return _enemyConfigs.ContainsKey(id);
        }

        public IEnumerable<EnemyConfig> GetEnemyConfigs(IEnumerable<string> ids)
        {
            return ids.Select(GetEnemyConfig).Where(config => config != null);
        }
        #endregion

        #region Enemy Spawn Group Config Management
        const string SpawnGroupConfigPath = "Preset/JSON";
        const string SpawnGroupConfigName = "EnemySpawnGroups.json";

        bool _spawnGroupConfigLoaded = false;
        readonly Dictionary<string, EnemySpawnGroupConfig> _spawnGroupConfigs = new();

        void LoadSpawnGroupConfigs()
        {
            if (_spawnGroupConfigLoaded) return;

            _spawnGroupConfigs.Clear();
            List<EnemySpawnGroupConfig> configs = SaveLoadManager.Load<List<EnemySpawnGroupConfig>>(SpawnGroupConfigName, SpawnGroupConfigPath);

            if (configs != null)
            {
                foreach (EnemySpawnGroupConfig config in configs)
                {
                    _spawnGroupConfigs[config.ID] = config;
                }
                Debug.Log($"Loaded {_spawnGroupConfigs.Count} enemy spawn group configs");
            }
            else
            {
                Debug.LogWarning($"Failed to load enemy spawn group configs from {SpawnGroupConfigName}");
            }

            _spawnGroupConfigLoaded = true;
        }

        public EnemySpawnGroupConfig GetSpawnGroupConfig(string id)
        {
            LoadSpawnGroupConfigs();

            if (_spawnGroupConfigs.TryGetValue(id, out EnemySpawnGroupConfig config))
            {
                return config;
            }

            Debug.LogWarning($"Enemy spawn group config not found: {id}");
            return null;
        }

        public List<EnemySpawnGroupConfig> GetAllSpawnGroupConfigs()
        {
            LoadSpawnGroupConfigs();
            return _spawnGroupConfigs.Values.ToList();
        }

        public bool HasSpawnGroupConfig(string id)
        {
            LoadSpawnGroupConfigs();
            return _spawnGroupConfigs.ContainsKey(id);
        }

        public IEnumerable<EnemySpawnGroupConfig> GetSpawnGroupConfigs(IEnumerable<string> ids)
        {
            return ids.Select(GetSpawnGroupConfig).Where(config => config != null);
        }
        #endregion

        #region Enemy Spawn Management
        /// <summary>
        /// 根据生成组配置选择一个随机敌人类型
        /// </summary>
        public string SelectRandomEnemyType(string spawnGroupID)
        {
            var spawnGroup = GetSpawnGroupConfig(spawnGroupID);
            return spawnGroup?.GetRandomEnemyID();
        }

        /// <summary>
        /// 检查指定敌人类型是否可以生成
        /// </summary>
        public bool CanSpawnEnemy(string spawnGroupID, string enemyID, Dictionary<string, int> currentCounts)
        {
            var spawnGroup = GetSpawnGroupConfig(spawnGroupID);
            if (spawnGroup == null) return false;

            if (!currentCounts.TryGetValue(enemyID, out int currentCount))
                return false;

            int maxCount = spawnGroup.GetMaxCountForEnemy(enemyID);
            return currentCount < maxCount;
        }

        /// <summary>
        /// 获取生成组的总体配置信息
        /// </summary>
        public (int totalMaxCount, float generateGap, int generateCount, float minDistance, float maxDistance)
            GetSpawnGroupInfo(string spawnGroupID)
        {
            var config = GetSpawnGroupConfig(spawnGroupID);
            if (config == null)
                return (0, 0, 0, 0, 0);

            return (config.TotalMaxCount, config.GenerateGap, config.GenerateCount,
                    config.MinDistance, config.MaxDistance);
        }

        /// <summary>
        /// 获取敌人的预制体地址
        /// </summary>
        public string GetEnemyPrefabAddress(string enemyID)
        {
            var config = GetEnemyConfig(enemyID);
            return config?.PrefabAddress;
        }

        /// <summary>
        /// 验证生成组中的所有敌人配置是否有效
        /// </summary>
        public bool ValidateSpawnGroup(string spawnGroupID)
        {
            var spawnGroup = GetSpawnGroupConfig(spawnGroupID);
            if (spawnGroup == null) return false;

            foreach (var entry in spawnGroup.EnemyEntries)
            {
                if (!HasEnemyConfig(entry.EnemyID))
                {
                    Debug.LogError($"Invalid enemy ID in spawn group {spawnGroupID}: {entry.EnemyID}");
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Configuration Reload
        /// <summary>
        /// 重新加载所有敌人配置
        /// </summary>
        public void ReloadAllConfigs()
        {
            ReloadEnemyConfigs();
            ReloadSpawnGroupConfigs();
        }

        /// <summary>
        /// 重新加载敌人配置
        /// </summary>
        public void ReloadEnemyConfigs()
        {
            _enemyConfigs.Clear();
            _enemyConfigLoaded = false;
            LoadEnemyConfigs();
        }

        /// <summary>
        /// 重新加载生成组配置
        /// </summary>
        public void ReloadSpawnGroupConfigs()
        {
            _spawnGroupConfigs.Clear();
            _spawnGroupConfigLoaded = false;
            LoadSpawnGroupConfigs();
        }
        #endregion

        #region System Lifecycle
        protected override void OnInit()
        {
            Debug.Log("EnemySystem initialized");
            // 预加载配置
            LoadEnemyConfigs();
            LoadSpawnGroupConfigs();
        }

        protected override void OnDeinit()
        {
            _enemyConfigs.Clear();
            _spawnGroupConfigs.Clear();
            _enemyConfigLoaded = false;
            _spawnGroupConfigLoaded = false;
            Debug.Log("EnemySystem deinitialized");
        }
        #endregion
    }
}
