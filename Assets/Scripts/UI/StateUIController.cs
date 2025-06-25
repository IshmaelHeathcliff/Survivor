using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Character.State;
using UnityEngine;

namespace UI
{
    public interface IStateUIController
    {
        void AddState(IState state);
        void RemoveState(string id);
        void ChangeStateTime(IStateWithTime state);
        void ChangeStateCount(IStateWithCount state);
    }

    [RequireComponent(typeof(StateUIPool))]
    public abstract class StateUIController : MonoBehaviour, IController, IStateUIController
    {
        protected IStateContainer StateContainer;

        protected abstract void SetStateContainer();

        readonly Dictionary<string, StateUI> _stateUIs = new();
        [SerializeField] StateUIPool _pool;

        async UniTaskVoid AddStateAsync(IState state)
        {
            StateUI stateUI = await _pool.Pop();

            if (_stateUIs.ContainsKey(state.GetID()))
            {
                RemoveState(state.GetID());
            }

            _stateUIs.Add(state.GetID(), stateUI);
            stateUI.InitStateUI(state);
        }

        public void AddState(IState state)
        {
            AddStateAsync(state).Forget();

        }

        public void RemoveState(string id)
        {
            if (_stateUIs.Remove(id, out StateUI stateUI))
            {
                _pool.Push(stateUI);
            }
        }

        public void ChangeStateTime(IStateWithTime state)
        {
            if (_stateUIs.TryGetValue(state.GetID(), out StateUI stateUI))
            {
                stateUI.SetTime(state.TimeLeft, state.Duration);
            }
        }

        public void ChangeStateCount(IStateWithCount state)
        {
            if (_stateUIs.TryGetValue(state.GetID(), out StateUI stateUI))
            {
                stateUI.SetCount(state.Count);
            }
        }

        void OnValidate()
        {
            _pool = GetComponent<StateUIPool>();
        }

        void Start()
        {
            SetStateContainer();
            StateContainer.OnStateAdded.Register(AddState);
            StateContainer.OnStateRemoved.Register(RemoveState);
            StateContainer.OnStateTimeChanged.Register(ChangeStateTime);
            StateContainer.OnStateCountChanged.Register(ChangeStateCount);
        }

        void FixedUpdate()
        {
            StateContainer.DecreaseStateTime(Time.fixedDeltaTime);
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
