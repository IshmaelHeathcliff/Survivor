using System.Collections.Generic;
using Sirenix.Utilities;

namespace Character.State
{
    public interface IStateContainer
    {
        EasyEvent<IState> OnStateAdded { get; }
        EasyEvent<string> OnStateRemoved { get; }
        EasyEvent<IStateWithTime> OnStateTimeChanged { get; }
        EasyEvent<IStateWithCount> OnStateCountChanged { get; }
        void AddState(IState state);
        void RemoveState(string id);
        void RemoveState(IState state);
        bool HasState(string id);
        bool HasState(IState state);
        void ResetStateTime(float time);
        void DecreaseStateTime(float time);
        void Clear();
    }


    public class StateContainer : IStateContainer
    {
        readonly Dictionary<string, IState> _states = new();
        public EasyEvent<IState> OnStateAdded { get; } = new();
        public EasyEvent<string> OnStateRemoved { get; } = new();
        public EasyEvent<IStateWithTime> OnStateTimeChanged { get; } = new();
        public EasyEvent<IStateWithCount> OnStateCountChanged { get; } = new();

        public void AddState(IState state)
        {
            if (_states.ContainsKey(state.GetID()))
            {
                // 如果状态已经存在，则更新状态而不是添加状态
                if (state is not IStateWithCount)
                {
                    RemoveState(state);
                }
                else
                {
                    // TODO：处理State的叠层
                    return;
                }
            }

            _states.Add(state.GetID(), state);
            state.Enable();
            OnStateAdded?.Trigger(state);

        }

        public bool HasState(string id)
        {
            return _states.ContainsKey(id);
        }

        public bool HasState(IState state)
        {
            return HasState(state.GetID());
        }

        public void RemoveState(IState state)
        {
            RemoveState(state.GetID());
        }

        public void RemoveState(string id)
        {
            if (HasState(id))
            {
                _states[id].Disable();
                _states.Remove(id);
                OnStateRemoved?.Trigger(id);
            }
        }

        public void ResetStateTime(float time)
        {
            foreach (IState state in _states.Values)
            {
                if (state is IStateWithTime bt)
                {
                    bt.ResetTime();
                    OnStateTimeChanged?.Trigger(bt);
                }
            }
        }

        public void DecreaseStateTime(float time)
        {

            List<IState> states = new();
            _states.Values.ForEach(state => states.Add(state));
            foreach (IState state in states)
            {
                if (state is not IStateWithTime st)
                {
                    continue;
                }

                st.DecreaseTime(time);
                OnStateTimeChanged?.Trigger(st);
                if (st.TimeLeft <= 0)
                {
                    RemoveState(state);
                }
            }
        }

        public void ChangeStateCount(string id, int count)
        {
            if (_states.TryGetValue(id, out IState state))
            {
                if (state is IStateWithCount bc)
                {
                    bc.Count = count;
                    OnStateCountChanged?.Trigger(bc);
                }
            }
        }

        public void Clear()
        {
            foreach (IState state in _states.Values)
            {
                state.Disable();
            }
            _states.Clear();
        }

    }
}
