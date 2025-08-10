using XYZRPGSystem.Core;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;
using System.Threading;
using System;

namespace Gameplay.Character.Enemy
{
    public class EnemyDeadState : EnemyState
    {
        public EnemyDeadState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        CancellationTokenSource _cts;

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

            _cts = GlobalCancellation.GetCombinedTokenSource(MoveController);
            try
            {
                await MoveController.PlayAnimation(EnemyMoveController.Dead, _cts.Token);
                Target.Destroy();
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
