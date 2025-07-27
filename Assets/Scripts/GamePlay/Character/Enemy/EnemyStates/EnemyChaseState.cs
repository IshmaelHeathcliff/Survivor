using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;

namespace Gameplay.Character.Enemy
{
    public class EnemyChaseState : EnemyState
    {
        public EnemyChaseState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        float _attackRange;



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
            MoveController.PlayAnimation(EnemyMoveController.Chase).Forget();
            _attackRange = Target.CharacterModel.Stats.GetStat("AttackRange").Value;

        }

        protected override void OnUpdate()
        {
        }

        protected override void OnFixedUpdate()
        {
            MoveController.FindPlayer();
            MoveController.Move();
            CheckPlayer();
        }
    }
}
