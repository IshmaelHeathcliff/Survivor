using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;


using Gameplay.Character.Player;
using XYZRPGSystem.Core;
using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Gameplay;
using XYZRPGSystem.Gameplay.Skill;

namespace Gameplay.Character.Enemy
{
    public class EnemyFactory : MonoBehaviour, IController
    {
        [Header("敌人生成组配置")]
        [SerializeField] string _spawnGroupID = "basic_spawn_group";

        EnemySpawnGroupConfig _spawnGroupConfig;
        Dictionary<string, EnemyConfig> _enemyConfigs = new();
        Dictionary<string, AssetReference> _enemyReferences = new();
        Dictionary<string, CancellationTokenSource> _enemySpawnTokens = new();

        void LoadConfig()
        {
            EnemySystem enemySystem = this.GetSystem<EnemySystem>();

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

            foreach (var entry in _spawnGroupConfig.EnemyEntries)
            {
                var enemyConfig = enemySystem.GetEnemyConfig(entry.EnemyID);
                if (enemyConfig != null)
                {
                    _enemyConfigs[entry.EnemyID] = enemyConfig;
                    _enemyReferences[entry.EnemyID] = new AssetReference(enemyConfig.PrefabAddress);
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
        async UniTask CreateEnemy(EnemySpawnEntry spawnEntry, Vector3 position, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string enemyID = spawnEntry.EnemyID;

            if (!_enemyReferences.TryGetValue(enemyID, out AssetReference enemyReference))
            {
                Debug.LogError($"Enemy reference not found: {enemyID}");
                return;
            }

            var obj = await Addressables.InstantiateAsync(enemyReference, transform).ToUniTask(cancellationToken: ct);

            obj.transform.position = position;

            // 初始化敌人属性
            InitializeEnemyStats(obj, enemyID);

            await UniTask.DelayFrame(2, cancellationToken: ct);

            foreach (string skillID in _enemyConfigs[enemyID].SkillIDs)
            {
                this.SendCommand(new AcquireSkillCommand(skillID, obj.GetComponent<EnemyController>().CharacterModel));
            }

        }

        /// <summary>
        /// 根据配置初始化敌人属性
        /// </summary>
        void InitializeEnemyStats(GameObject enemyObj, string enemyID)
        {
            if (!_enemyConfigs.TryGetValue(enemyID, out EnemyConfig enemyConfig))
            {
                Debug.LogError($"Enemy config not found for: {enemyID}");
                return;
            }

            // 获取敌人控制器组件
            if (!enemyObj.TryGetComponent(out EnemyController enemyController))
            {
                Debug.LogError($"EnemyController component not found on enemy: {enemyID}");
                return;
            }

            // 初始化控制器
            enemyController.Init();

            // 设置基础属性
            enemyController.SetConsumableStat("Health", enemyConfig.Health, true);
            enemyController.SetStat("HealthRegen", enemyConfig.HealthRegen);
            enemyController.SetStat("MoveSpeed", enemyConfig.MoveSpeed);
            enemyController.SetStat("Damage", enemyConfig.Damage);
            enemyController.SetStat("CooldownInverse", enemyConfig.CooldownInverse);
            enemyController.SetStat("AttackSpeed", enemyConfig.AttackSpeed);
            enemyController.SetStat("AttackRange", enemyConfig.AttackRange);
            enemyController.SetStat("CoinOnDead", enemyConfig.CoinOnDead);
            enemyController.SetStat("WoodOnDead", enemyConfig.WoodOnDead);
            enemyController.SetStat("HealthIncreasePerWave", enemyConfig.HealthIncreasePerWave);
            enemyController.SetStat("DamageIncreasePerWave", enemyConfig.DamageIncreasePerWave);
        }

        /// <summary>
        /// 为单个敌人类型独立生成的协程
        /// </summary>
        async UniTask ProduceEnemy(EnemySpawnEntry spawnEntry, CancellationToken ct)
        {
            // 获取生成配置
            float generateGap = spawnEntry.GenerateGap;
            int generateCount = spawnEntry.GenerateCount;

            try
            {
                while (true)
                {
                    ct.ThrowIfCancellationRequested();

                    if (this.GetModel<EnemiesModel>().GetCount() < _spawnGroupConfig.TotalMaxCount)
                    {
                        // 生成指定数量的敌人
                        for (int i = 0; i < generateCount; i++)
                        {
                            await CreateEnemy(spawnEntry, GetRandomPosition(), ct);
                        }
                    }

                    // 等待下次生成
                    await UniTask.Delay((int)(generateGap * 1000), cancellationToken: ct);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Enemy spawn for {spawnEntry.EnemyID} is canceled");
            }
        }

        /// <summary>
        /// 启动所有敌人类型的生成
        /// </summary>
        [Button("启动所有敌人生成")]
        void StartSpawning()
        {
            // 停止之前的生成协程
            StopAllSpawning();

            CancellationToken globalCt = GlobalCancellation.GetCombinedTokenSource(this).Token;

            foreach (EnemySpawnEntry entry in _spawnGroupConfig.EnemyEntries)
            {
                if (!string.IsNullOrEmpty(entry.EnemyID))
                {
                    // 为每个敌人类型创建独立的取消令牌
                    var cts = CancellationTokenSource.CreateLinkedTokenSource(globalCt);
                    _enemySpawnTokens[entry.EnemyID] = cts;

                    // 启动独立的生成协程
                    ProduceEnemy(entry, cts.Token).Forget();

                    Debug.Log($"Started independent spawning for enemy: {entry.EnemyID}, Gap: {entry.GenerateGap}s, Count: {entry.GenerateCount}");
                }
            }
        }

        /// <summary>
        /// 停止所有敌人生成
        /// </summary>
        [Button("停止所有敌人生成")]
        void StopAllSpawning()
        {
            foreach ((_, CancellationTokenSource cts) in _enemySpawnTokens)
            {
                cts?.Cancel();
                cts?.Dispose();
            }
            _enemySpawnTokens.Clear();
        }

        /// <summary>
        /// 切换敌人生成组配置
        /// </summary>
        [Button("切换敌人生成组配置")]
        public void SwitchSpawnGroup(string newSpawnGroupID)
        {
            _spawnGroupID = newSpawnGroupID;
            LoadConfig();
            if (_spawnGroupConfig != null)
            {
                StartSpawning();
            }
        }

        /// <summary>
        /// 停止特定敌人类型的生成
        /// </summary>
        [Button("停止特定敌人生成")]
        public void StopSpawning(string enemyID)
        {
            if (_enemySpawnTokens.TryGetValue(enemyID, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
                _enemySpawnTokens.Remove(enemyID);
                Debug.Log($"Stopped spawning for enemy: {enemyID}");
            }
        }

        /// <summary>
        /// 重新启动特定敌人类型的生成
        /// </summary>
        [Button("重新启动特定敌人生成")]
        public void RestartSpawning(string enemyID)
        {
            var entry = _spawnGroupConfig.EnemyEntries.Find(e => e.EnemyID == enemyID);
            if (entry != null)
            {
                // 停止现有的生成
                StopSpawning(enemyID);

                // 启动新的生成
                CancellationToken globalCt = GlobalCancellation.GetCombinedTokenSource(this).Token;
                var cts = CancellationTokenSource.CreateLinkedTokenSource(globalCt);
                _enemySpawnTokens[enemyID] = cts;

                ProduceEnemy(entry, cts.Token).Forget();
                Debug.Log($"Restarted spawning for enemy: {enemyID}");
            }
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

            StartSpawning();
        }

        void OnDestroy()
        {
            StopAllSpawning();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
