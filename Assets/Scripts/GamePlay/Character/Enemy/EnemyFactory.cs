using System;
using System.Collections.Generic;
using System.Threading;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Gameplay.Character.Player;
using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Gameplay;
using Random = UnityEngine.Random;

namespace Gameplay.Character.Enemy
{
    public class EnemyFactory : MonoBehaviour, IController
    {
        [Header("敌人生成组配置")]
        [SerializeField] string _spawnGroupID = "basic_spawn_group";

        EnemySpawnGroupConfig _spawnGroupConfig;
        Dictionary<string, EnemyConfig> _enemyConfigs = new();
        Dictionary<string, AssetReference> _enemyReferences = new();
        Dictionary<string, int> _currentEnemyCounts = new();

        void LoadConfig()
        {
            var enemySystem = this.GetSystem<EnemySystem>();

            _spawnGroupConfig = enemySystem.GetSpawnGroupConfig(_spawnGroupID);
            if (_spawnGroupConfig == null)
            {
                Debug.LogError($"Enemy spawn group config not found: {_spawnGroupID}");
                return;
            }

            // 验证生成组配置有效性
            if (!enemySystem.ValidateSpawnGroup(_spawnGroupID))
            {
                Debug.LogError($"Invalid spawn group configuration: {_spawnGroupID}");
                return;
            }

            // 加载所有敌人配置和资源引用
            _enemyConfigs.Clear();
            _enemyReferences.Clear();
            _currentEnemyCounts.Clear();

            foreach (var entry in _spawnGroupConfig.EnemyEntries)
            {
                var enemyConfig = enemySystem.GetEnemyConfig(entry.EnemyID);
                if (enemyConfig != null)
                {
                    _enemyConfigs[entry.EnemyID] = enemyConfig;
                    _enemyReferences[entry.EnemyID] = new AssetReference(enemyConfig.PrefabAddress);
                    _currentEnemyCounts[entry.EnemyID] = 0;
                }
                else
                {
                    Debug.LogWarning($"Enemy config not found: {entry.EnemyID}");
                }
            }
        }

        /// <summary>
        /// 创建Enemy实例
        /// </summary>
        async UniTask CreateEnemy(string enemyID, Vector3 position, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            if (!_enemyReferences.TryGetValue(enemyID, out AssetReference enemyReference))
            {
                Debug.LogError($"Enemy reference not found: {enemyID}");
                return;
            }

            var obj = await Addressables.InstantiateAsync(enemyReference, transform).ToUniTask(cancellationToken: ct);
            obj.transform.position = position;

            // 更新当前敌人数量
            _currentEnemyCounts[enemyID]++;
        }

        async UniTask ProduceEnemies()
        {
            CancellationToken ct = GlobalCancellation.GetCombinedTokenSource(this).Token;

            try
            {
                while (true)
                {
                    if (transform.childCount < _spawnGroupConfig.TotalMaxCount)
                    {
                        // 生成配置中指定数量的敌人
                        for (int i = 0; i < _spawnGroupConfig.GenerateCount; i++)
                        {
                            string enemyID = _spawnGroupConfig.GetRandomEnemyID();
                            if (!string.IsNullOrEmpty(enemyID) && CanSpawnEnemy(enemyID))
                            {
                                await CreateEnemy(enemyID, GetRandomPosition(), ct);
                            }
                        }
                    }

                    await UniTask.Delay((int)(_spawnGroupConfig.GenerateGap * 1000), cancellationToken: ct); // ms
                }
            }
            catch (OperationCanceledException)
            {
                // Debug.Log("EnemyFactory is canceled");
            }
        }

        bool CanSpawnEnemy(string enemyID)
        {
            if (!_currentEnemyCounts.TryGetValue(enemyID, out int currentCount))
                return false;

            int maxCount = _spawnGroupConfig.GetMaxCountForEnemy(enemyID);
            return currentCount < maxCount;
        }

        /// <summary>
        /// 当敌人被销毁时调用，更新敌人数量计数
        /// </summary>
        public void OnEnemyDestroyed(string enemyID)
        {
            if (_currentEnemyCounts.ContainsKey(enemyID))
            {
                _currentEnemyCounts[enemyID] = Mathf.Max(0, _currentEnemyCounts[enemyID] - 1);
            }
        }

        /// <summary>
        /// 切换敌人生成组配置
        /// </summary>
        public void SwitchSpawnGroup(string newSpawnGroupID)
        {
            _spawnGroupID = newSpawnGroupID;
            LoadConfig();
        }

        /// <summary>
        /// 获取当前敌人数量信息
        /// </summary>
        public Dictionary<string, int> GetCurrentEnemyCounts()
        {
            return new Dictionary<string, int>(_currentEnemyCounts);
        }

        Vector2 GetRandomPosition()
        {
            Vector2 playerPosition = this.SendQuery(new PlayerPositionQuery());
            float angle = Random.Range(0, 2 * Mathf.PI);
            var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 randomPosition = playerPosition + randomDirection * Random.Range(_spawnGroupConfig.MinDistance, _spawnGroupConfig.MaxDistance);
            return randomPosition;
        }

        void Start()
        {
            LoadConfig();
            if (_spawnGroupConfig == null) return;

            ProduceEnemies().Forget();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
