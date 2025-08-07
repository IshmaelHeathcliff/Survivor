using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Data.Config;
using Gameplay.Character.Player;

using XYZRPGSystem.Data.SaveLoad;
using XYZRPGSystem.Gameplay.Item;

namespace Gameplay.Level
{
    public class LevelSystem : AbstractSystem
    {
        const string PresetPath = "Preset/JSON";
        const string LevelsConfigPath = "Levels.json";

        readonly Dictionary<string, LevelConfig> _levelConfigs = new();
        LevelModel _levelModel;
        readonly List<IUnRegister> _unRegisters = new();

        protected override void OnInit()
        {
            LoadLevelConfigs();
            _levelModel = this.GetModel<LevelModel>();

            this.RegisterEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        protected override void OnDeinit()
        {
            _unRegisters.ForEach(unRegister => unRegister.UnRegister());
            _unRegisters.Clear();

            this.UnRegisterEvent<LevelCompletedEvent>(OnLevelCompleted);
        }

        void LoadLevelConfigs()
        {
            try
            {
                List<LevelConfig> configs = SaveLoadManager.Load<List<LevelConfig>>(LevelsConfigPath, PresetPath);
                if (configs != null)
                {
                    _levelConfigs.Clear();

                    foreach (LevelConfig config in configs)
                    {
                        _levelConfigs[config.ID] = config;
                    }

                    // Debug.Log($"已加载 {_levelConfigs.Count} 个关卡配置");
                }
                else
                {
                    Debug.LogError("未找到关卡配置文件");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载关卡配置失败: {e.Message}");
            }
        }

        public void LoadLevel(string levelID)
        {
            if (!_levelConfigs.TryGetValue(levelID, out LevelConfig levelConfig))
            {
                Debug.LogError($"找不到关卡配置: {levelID}");
                return;
            }

            _levelModel.SetLevel(levelConfig);

            // Debug.Log($"已加载关卡: {levelConfig.Name}");
        }

        void OnLevelCompleted(LevelCompletedEvent e)
        {
            if (e.Success)
            {
                if (!string.IsNullOrEmpty(e.NextLevelID))
                {
                    LoadLevel(e.NextLevelID);
                }
            }
        }

        public void CompleteLevel(bool success)
        {
            if (_levelModel == null || _levelModel.ID == null)
            {
                Debug.LogError("当前关卡为空");
                return;
            }

            if (success)
            {
                Debug.Log($"关卡 {_levelModel.Name} 完成！");
            }
            else
            {
                Debug.Log($"关卡 {_levelModel.Name} 失败");
            }

            _levelModel.CompleteLevel(success);
        }

        public bool NextWave()
        {
            if (_levelModel.CurrentWave != null)
            {
                GiveRewards(_levelModel.CurrentWave.Rewards);
            }

            return _levelModel.NextWave();
        }


        void GiveRewards(List<LevelReward> rewards)
        {
            ResourceSystem resourceSystem = this.GetSystem<ResourceSystem>();
            PlayerModel playerModel = this.GetModel<PlayersModel>().Current;
            foreach (LevelReward reward in rewards)
            {
                switch (reward.Type)
                {
                    case LevelReward.RewardType.Coin:
                        resourceSystem.AcquireResource(ResourceType.Coin, reward.Amount, playerModel);
                        break;
                    case LevelReward.RewardType.Wood:
                        resourceSystem.AcquireResource(ResourceType.Wood, reward.Amount, playerModel);
                        break;
                }
            }
        }
    }
}
