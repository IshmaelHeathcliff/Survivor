using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Character.Enemy
{
    public class EnemyIdleState : EnemyState
    {
        float _idleTime;
        public EnemyIdleState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateID.Dead;
        }

        protected override void OnFixedUpdate()
        {
            if (_idleTime > 0)
            {
                _idleTime -= Time.fixedDeltaTime;
            }
            else
            {
                FSM.ChangeState(EnemyStateID.Patrol);
            }
        }

        protected override void OnEnter()
        {
            _idleTime = MoveController.IdleTime;
            MoveController.PlayAnimation(EnemyMoveController.Idle).Forget();
            MoveController.Freeze();
        }
    }
}
