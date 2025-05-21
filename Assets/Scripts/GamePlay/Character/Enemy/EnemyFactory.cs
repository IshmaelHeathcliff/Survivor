using Character.Enemy;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Character
{
    public class EnemyFactory : MonoBehaviour, IController
    {
        [SerializeField] GameObject _enemyPrefab;
        [Header("生成信息")]
        [SerializeField] int _maxCount;
        [SerializeField] float _generateGap;
        [SerializeField] float _minDistance;
        [SerializeField] float _maxDistance;

        EnemiesModel _model;

        /// <summary>
        /// 创建EnemyModel，将EnemyModel的相关依赖注入到EnemiesModel中
        /// </summary>
        void CreateEnemy(Vector3 position)
        {
            string id = GUID.Generate().ToString();
            _model.AddModel(id, new EnemyModel());
            _enemyPrefab.GetComponent<EnemyController>().ID = id;
            GameObject obj = Instantiate(_enemyPrefab, transform);
            obj.transform.position = position;
        }

        async UniTask ProduceEnemies()
        {
            while (transform.childCount < _maxCount)
            {
                CreateEnemy(GetRandomPosition());
                await UniTask.Delay((int)(_generateGap * 1000)); // ms
            }
        }

        Vector3 GetRandomPosition()
        {
            Vector3 playerPosition = this.SendQuery(new PlayerPositionQuery());
            var angle = Random.Range(0, 2 * Mathf.PI);
            var randomDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Vector3 randomPosition = playerPosition + randomDirection * Random.Range(_minDistance, _maxDistance);
            return randomPosition;
        }

        void Awake()
        {
            _model = this.GetModel<EnemiesModel>();
        }

        void Start()
        {
            // _enemyPrefab = await AddressablesManager.LoadAsset<GameObject>("Enemy101");
            ProduceEnemies().Forget();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
