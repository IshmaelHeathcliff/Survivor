using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public class StateUIPool : MonoBehaviour, IAsyncObjectPool<StateUI>
    {
        [SerializeField] AssetReferenceGameObject _stateUIReference;
        [SerializeField] int _initialSize = 10;
        [SerializeField] int _maxSize = 100;
        readonly Stack<StateUI> _pool = new();

        public int Count => _pool.Count;

        async UniTask<StateUI> CreatObject()
        {
            GameObject obj = await Addressables.InstantiateAsync(_stateUIReference, transform);
            obj.SetActive(false);
            return obj.GetOrAddComponent<StateUI>();
        }

        public async UniTask<StateUI> Pop()
        {
            StateUI stateUI;
            if (Count > 0)
            {
                stateUI = _pool.Pop();
            }
            else
            {
                stateUI = await CreatObject();
            }

            stateUI.gameObject.SetActive(true);
            return stateUI;
        }

        public void Push(StateUI stateUI)
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
            foreach (StateUI stateUI in _pool)
            {
                Addressables.ReleaseInstance(stateUI.gameObject);
            }

            _pool.Clear();
        }
    }
}
