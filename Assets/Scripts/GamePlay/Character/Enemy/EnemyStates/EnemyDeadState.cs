using Core;

namespace Character.Enemy
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
            Target.AttackerController.CanAttack = false;
            await MoveController.PlayAnimation(EnemyMoveController.Dead).SuppressCancellationThrow();
            Target.Destroy();
        }
    }
}
