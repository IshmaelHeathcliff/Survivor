using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Character.State
{
    public class StateUICellPool : MonoBehaviour, IAsyncObjectPool<StateUICell>
    {
        [SerializeField] AssetReferenceGameObject _stateUICellReference;
        [SerializeField] int _initialSize = 10;
        [SerializeField] int _maxSize = 100;
        readonly Stack<StateUICell> _pool = new();

        public int Count => _pool.Count;

        async UniTask<StateUICell> CreatObject()
        {
            GameObject obj = await Addressables.InstantiateAsync(_stateUICellReference);
            obj.SetActive(false);
            return obj.GetOrAddComponent<StateUICell>();
        }

        public async UniTask<StateUICell> Pop()
        {
            StateUICell stateUICell;
            if (Count > 0)
            {
                stateUICell = _pool.Pop();
            }
            else
            {
                stateUICell = await CreatObject();
            }

            stateUICell.gameObject.SetActive(true);
            return stateUICell;
        }

        public void Push(StateUICell stateUI)
        {
            stateUI.gameObject.SetActive(false);
            if (Count > _maxSize)
            {
                Addressables.ReleaseInstance(stateUI.gameObject);
                return;
            }
            _pool.Push(stateUI);
        }

        async UniTaskVoid Init()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                _pool.Push(await CreatObject());
            }
        }

        void OnEnable()
        {
            Init().Forget();
        }

        void OnDisable()
        {
            foreach (StateUICell stateUI in _pool)
            {
                Addressables.ReleaseInstance(stateUI.gameObject);
            }

            _pool.Clear();
        }
    }
}
