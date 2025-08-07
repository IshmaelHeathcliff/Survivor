using System.Collections.Generic;
using System.Linq;
using Data.Config;

namespace Gameplay.Level
{
    public class LevelModel : AbstractModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LevelNumber { get; set; }
        public string NextLevelID { get; set; }
        public List<LevelWave> Waves { get; set; } = new List<LevelWave>();

        LevelWave _currentWave;
        public LevelWave CurrentWave
        {
            get => _currentWave;
            set
            {
                _currentWave = value;
                this.SendEvent(new LevelWaveStartEvent { LevelID = ID, LevelWave = value });
            }
        }


        protected override void OnInit()
        {
        }

        public void SetLevel(LevelConfig levelConfig)
        {
            ID = levelConfig.ID;
            Name = levelConfig.Name;
            Description = levelConfig.Description;
            LevelNumber = levelConfig.LevelNumber;

            foreach (LevelWaveConfig waveConfig in levelConfig.WaveConfigs)
            {
                var wave = new LevelWave
                {
                    WaveNumber = waveConfig.WaveNumber,
                    EnemySpawnGroupID = waveConfig.EnemySpawnGroupID,
                    EnemySpawnRateMultiplier = waveConfig.EnemySpawnRateMultiplier,
                    EnemyHealthMultiplier = waveConfig.EnemyHealthMultiplier,
                    EnemyDamageMultiplier = waveConfig.EnemyDamageMultiplier,
                    MaxEnemyCount = waveConfig.MaxEnemyCount,
                    SkillRarityWeights = waveConfig.SkillRarityWeights,
                    Rewards = waveConfig.Rewards,
                    Duration = waveConfig.Duration,
                    KillTarget = waveConfig.KillTarget
                };
                Waves.Add(wave);
            }

            Waves.Sort((w1, w2) => w1.WaveNumber.CompareTo(w2.WaveNumber));
            CurrentWave = Waves.First();
            this.SendEvent(new LevelStartedEvent { LevelID = levelConfig.ID });
        }

        public bool NextWave()
        {
            int currentIndex = Waves.IndexOf(CurrentWave);
            if (currentIndex + 1 >= Waves.Count)
            {
                CompleteLevel(true);
                return false;
            }

            CurrentWave = Waves[currentIndex + 1];
            return true;
        }

        public void CompleteLevel(bool success)
        {
            this.SendEvent(new LevelCompletedEvent { LevelID = ID, NextLevelID = NextLevelID, Success = success });
        }
    }

    public class LevelWave
    {
        public int WaveNumber { get; set; }
        public string EnemySpawnGroupID { get; set; }
        public float EnemySpawnRateMultiplier { get; set; } = 1.0f;
        public float EnemyHealthMultiplier { get; set; } = 1.0f;
        public float EnemyDamageMultiplier { get; set; } = 1.0f;
        public int MaxEnemyCount { get; set; } = 100;

        public List<SkillRarityWeight> SkillRarityWeights { get; set; } = new List<SkillRarityWeight>();

        public List<LevelReward> Rewards { get; set; } = new List<LevelReward>();

        public float Duration { get; set; } = 60f;
        public int KillTarget { get; set; } = 0;
    }

    public struct LevelStartedEvent
    {
        public string LevelID;
    }

    public struct LevelWaveStartEvent
    {
        public string LevelID;
        public LevelWave LevelWave;
    }

    public struct LevelCompletedEvent
    {
        public string LevelID;
        public string NextLevelID;
        public bool Success;
    }
}
