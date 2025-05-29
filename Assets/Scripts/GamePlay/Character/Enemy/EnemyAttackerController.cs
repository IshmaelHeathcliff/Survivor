using Character.Damage;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;

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

        protected override UniTask<IAttacker> GetOrCreateAttackerInternalAsync()
        {
            EnemyAttacker attacker = GetComponentInChildren<EnemyAttacker>();
            attacker.SetStats(CharacterController.Stats);
            return UniTask.FromResult<IAttacker>(attacker);
        }

        void Start()
        {
            GetOrCreateAttackerAsync().Forget();
        }
    }
}
