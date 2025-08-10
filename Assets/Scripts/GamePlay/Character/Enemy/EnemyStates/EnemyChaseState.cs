using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Gameplay.Character.Enemy
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        float _attackRange;
        CancellationTokenSource _cts;

        void CheckPlayer()
        {
            if (MoveController.SqrDistanceToPlayer() < _attackRange * _attackRange)
            {
                FSM.ChangeState(EnemyStateID.Attack);
            }
        }

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateID.Idle;
        }

        protected override void OnEnter()
        {
            _attackRange = Target.CharacterModel.Stats.GetStat("AttackRange").Value;

            _cts = GlobalCancellation.GetCombinedTokenSource(MoveController);
            try
            {
                MoveController.PlayAnimation(EnemyMoveController.Chase, _cts.Token).Forget();
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnFixedUpdate()
        {
            MoveController.FindPlayer();
            MoveController.Move();
            CheckPlayer();
        }

        protected override void OnExit()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}
