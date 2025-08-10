using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
namespace Gameplay.Character.Enemy
{
    public class EnemyHurtState : EnemyState
    {
        public EnemyHurtState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        CancellationTokenSource _cts;

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateID.Dead;
        }

        protected async override void OnEnter()
        {
            Target.Damageable.IsDamageable = false;
            MoveController.Stop();

            if (Target.Damageable.Health.CurrentValue <= 0)
            {
                FSM.ChangeState(EnemyStateID.Dead);
            }
            else
            {
                _cts = GlobalCancellation.GetCombinedTokenSource(MoveController);
                try
                {
                    await MoveController.PlayAnimation(EnemyMoveController.Hurt, _cts.Token);
                    FSM.ChangeState(EnemyStateID.Idle);
                }
                catch (OperationCanceledException)
                {
                }
            }

        }

        protected override void OnExit()
        {
            Target.Damageable.IsDamageable = true;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
