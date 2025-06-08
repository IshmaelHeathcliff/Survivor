using Character.Damage;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;

namespace Character.Enemy
{
    public class EnemyAttackerController : AttackerController
    {
        FSM<EnemyStateID> _fsm;
        public FSM<EnemyStateID> FSM => _fsm;

        protected override void OnInit()
        {
            base.OnInit();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>).FSM;
        }

        protected override UniTask<IAttacker> GetOrCreateAttackerAsyncInternal(string address = null)
        {
            EnemyAttacker attacker = GetComponentInChildren<EnemyAttacker>();
            attacker.SetStats(CharacterController.Stats);
            return UniTask.FromResult<IAttacker>(attacker);
        }

        void Start()
        {
            CreateAttacker().Forget();
        }
    }
}
