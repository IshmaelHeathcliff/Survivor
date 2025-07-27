using XYZRPGSystem.Core;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;

namespace Gameplay.Character.Enemy
{
    public class EnemyDeadState : EnemyState
    {
        public EnemyDeadState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateID.Hurt;
        }

        protected async override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            Target.GetSystem<PositionQuerySystem>().UnregisterModel(Target.tag, Target.CharacterModel);
            Target.AttackerController.CanAttack = false;
            Target.Damageable.OnDeath.Trigger();
            await MoveController.PlayAnimation(EnemyMoveController.Dead);
            Target.Destroy();
        }
    }
}
