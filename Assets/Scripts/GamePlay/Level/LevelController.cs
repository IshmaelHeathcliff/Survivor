using UnityEngine;
using Cysharp.Threading.Tasks;

using XYZRPGSystem.Gameplay;
using System.Threading;

namespace Gameplay.Level
{
    public class LevelController : MonoBehaviour, IController
    {
        [SerializeField] string _levelID;
        LevelSystem _levelSystem;
        LevelModel _levelModel;
        EnemySystem _enemySystem;

        CancellationTokenSource _cts;

        void Awake()
        {
            _levelSystem = this.GetSystem<LevelSystem>();
            _levelModel = this.GetModel<LevelModel>();
            _enemySystem = this.GetSystem<EnemySystem>();

            _levelSystem.LoadLevel(_levelID);
            _cts = new CancellationTokenSource();
        }

        async UniTask LoadNextLevelWave(CancellationToken cancellationToken)
        {
            await UniTask.Delay((int)(_levelModel.CurrentWave.Duration * 1000), cancellationToken: cancellationToken);
            if (!_levelSystem.NextWave())
            {
                Debug.Log("关卡完成", this);
                _levelSystem.CompleteLevel(true);
            }
            else
            {
                Debug.Log($"当前波次：{_levelModel.CurrentWave.WaveNumber}", this);
                Debug.Log($"当前波次敌人生成组：{_levelModel.CurrentWave.EnemySpawnGroupID}", this);
                await LoadNextLevelWave(cancellationToken);
            }
        }

        void Start()
        {
            LoadNextLevelWave(_cts.Token).Forget();
        }

        void OnDestroy()
        {
            _cts.Cancel();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
