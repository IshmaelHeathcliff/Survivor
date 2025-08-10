using System;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Character.Enemy
{
    public class EnemyPatrolState : EnemyState
    {
        CancellationTokenSource _cts;

        public EnemyPatrolState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateID.Idle;
        }

        protected override void OnEnter()
        {
            _cts = GlobalCancellation.GetCombinedTokenSource(MoveController);
            try
            {
                MoveController.PlayAnimation(EnemyMoveController.Patrol, _cts.Token).Forget();
                ChangeDirection().Forget();
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnFixedUpdate()
        {
            if (MoveController.FindPlayer())
            {
                FSM.ChangeState(EnemyStateID.Chase);
                return;
            }

            MoveController.Move();
        }

        protected override void OnExit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        Vector2 RandomDirection()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            return new Vector2(x, y);
        }

        async UniTaskVoid ChangeDirection()
        {
            while (true)
            {
                MoveController.Face(RandomDirection());
                await UniTask.Delay((int)(Random.Range(1f, 3f) * 1000), cancellationToken: _cts.Token);
            }
        }
    }
}
