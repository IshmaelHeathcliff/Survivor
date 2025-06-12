using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Data.Config;
using Data.SaveLoad;
using GamePlay.Skill;

namespace GamePlay.Character.Damage
{
    public interface IAttackerCreateSystem : ISystem
    {
        UniTask<IEnumerable<IAttacker>> CreateAttacker(AttackSkill skill, string attackerID, Transform parent);
    }

    public class AttackerCreateSystem : AbstractSystem, IAttackerCreateSystem
    {
        const string JsonPath = "Preset";
        const string JsonName = "Attackers.json";

        readonly Dictionary<string, AttackerConfig> _attackerConfigs = new();

        void Load()
        {
            _attackerConfigs.Clear();
            List<AttackerConfig> configs = this.GetUtility<SaveLoadUtility>().Load<List<AttackerConfig>>(JsonName, JsonPath);
            foreach (AttackerConfig config in configs)
            {
                _attackerConfigs.Add(config.AttackerID, config);
            }
        }

        public AttackerConfig GetAttackerConfig(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError("获取Attacker配置失败：ID为空");
                return null;
            }

            if (_attackerConfigs.TryGetValue(id, out AttackerConfig config))
            {
                return config;
            }

            Debug.LogWarning($"未找到ID为{id}的Attacker配置");
            return null;
        }

        public async UniTask<IEnumerable<IAttacker>> CreateAttacker(AttackSkill skill, string attackerID, Transform parent)
        {
            try
            {
                List<IAttacker> attackers = new();
                AttackerConfig config = GetAttackerConfig(attackerID);
                if (config == null)
                {
                    Debug.LogError($"未找到ID为{attackerID}的Attacker配置");
                    return null;
                }

                for (int i = 0; i < skill.ProjectileCount.Value; i++)
                {
                    GameObject attackerObj = await Addressables.InstantiateAsync(config.Address, parent);
                    if (attackerObj.TryGetComponent(out IAttacker attacker))
                    {
                        attackers.Add(attacker);
                    }
                    else
                    {
                        Debug.LogError($"无法获取Attacker组件: {config.Address}");
                        Addressables.Release(attackerObj);
                        return null;
                    }
                }

                foreach (IAttacker attacker in attackers)
                {
                    attacker.SetSkill(skill);
                }

                return attackers;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        protected override void OnInit()
        {
            // 系统初始化逻辑
            Load();
        }
    }
}
