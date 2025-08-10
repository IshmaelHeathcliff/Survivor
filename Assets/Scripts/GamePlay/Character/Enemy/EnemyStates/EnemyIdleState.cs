using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using System;

namespace Gameplay.Character.Enemy
{
    public class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        CancellationTokenSource _cts;

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is not EnemyStateID.Dead;
        }

        protected async override void OnEnter()
        {
            _cts = GlobalCancellation.GetCombinedTokenSource(MoveController);
            MoveController.Stop();
            try
            {
                await MoveController.PlayAnimation(EnemyMoveController.Idle, _cts.Token);
                FSM.ChangeState(EnemyStateID.Chase);
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnExit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
