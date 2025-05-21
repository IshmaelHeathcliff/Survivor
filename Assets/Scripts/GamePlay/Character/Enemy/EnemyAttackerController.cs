using Character.Damage;
using UnityEngine;
using Core;

namespace Character.Enemy
{
    public class EnemyAttackerController : AttackerController
    {
        FSM<EnemyStateId> _fsm;
        public FSM<EnemyStateId> FSM => _fsm;

        protected override void OnInit()
        {
            base.OnInit();
            _fsm = (CharacterController as IHasFSM<EnemyStateId>).FSM;
        }

        protected override IAttacker GetOrCreateAttackerInternal()
        {
            EnemyAttacker attacker = GetComponentInChildren<EnemyAttacker>();
            attacker.SetStats(CharacterController.Stats);
            return attacker;
        }

        void Start()
        {
            GetOrCreateAttacker();
        }
    }
}
