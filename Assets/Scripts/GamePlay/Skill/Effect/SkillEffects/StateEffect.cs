using System.Collections.Generic;
using Data.Config;
using GamePlay.Character;
using GamePlay.Character.State;

namespace GamePlay.Skill.Effect
{
    public class StateEffect : SkillEffect<StateEffectConfig>
    {
        readonly StateCreateSystem _stateCreateSystem;
        IState _state;
        public StateEffect(StateEffectConfig skillEffectConfig, ICharacterModel model, StateCreateSystem stateCreateSystem) : base(skillEffectConfig, model)
        {
            _stateCreateSystem = stateCreateSystem;
        }

        protected override void OnApply()
        {
            _state = _stateCreateSystem.CreateState(SkillEffectConfig.StateID, Model.Stats, SkillEffectConfig.Values);
            Model.StateContainer.AddState(_state);
        }

        protected override void OnCancel()
        {
            Model.StateContainer.RemoveState(_state);
        }
    }

    public class StateWithTimeEffect : SkillEffect<StateWithTimeEffectConfig>
    {
        readonly StateCreateSystem _stateCreateSystem;
        IStateWithTime _state;
        public StateWithTimeEffect(StateWithTimeEffectConfig skillEffectConfig, ICharacterModel model, StateCreateSystem stateCreateSystem) : base(skillEffectConfig, model)
        {
            _stateCreateSystem = stateCreateSystem;
        }

        protected override void OnApply()
        {
            _state = _stateCreateSystem.CreateState(SkillEffectConfig.StateID, Model.Stats, SkillEffectConfig.Duration, SkillEffectConfig.Values);
            Model.StateContainer.AddState(_state);
        }

        protected override void OnCancel()
        {
            Model.StateContainer.RemoveState(_state);
        }

    }
}
