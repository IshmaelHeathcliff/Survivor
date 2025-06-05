using UnityEngine;

namespace Character.State
{
    public interface IStateUI
    {
        void AddState(IState state);
        void RemoveState(string id);
        void ChangeStateTime(IStateWithTime state);
        void ChangeStateCount(IStateWithCount state);
    }

    public abstract class StateUI : MonoBehaviour, IStateUI
    {
        public abstract void AddState(IState state);
        public abstract void RemoveState(string id);
        public abstract void ChangeStateTime(IStateWithTime state);
        public abstract void ChangeStateCount(IStateWithCount state);
    }

    public abstract class StateController : MonoBehaviour, IController
    {
        [SerializeField] StateUI _stateUI;
        protected IStateContainer StateContainer;


        void OnValidate()
        {
            if (_stateUI == null)
            {
                _stateUI = GetComponentInChildren<StateUI>();
            }
        }

        protected abstract void SetStateContainer();

        void Start()
        {
            SetStateContainer();
            StateContainer.OnStateAdded.Register(_stateUI.AddState);
            StateContainer.OnStateRemoved.Register(_stateUI.RemoveState);
            StateContainer.OnStateTimeChanged.Register(_stateUI.ChangeStateTime);
            StateContainer.OnStateCountChanged.Register(_stateUI.ChangeStateCount);
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
