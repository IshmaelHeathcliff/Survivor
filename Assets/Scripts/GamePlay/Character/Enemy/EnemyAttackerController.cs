using Character.Damage;
using UnityEngine;
using Core;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Character.Enemy
{
    public class EnemyAttackerController : AttackerController
    {
        FSM<EnemyStateID> _fsm;
        public FSM<EnemyStateID> FSM => _fsm;

        AttackerCreateSystem _attackerCreateSystem;

        protected override void OnInit()
        {
            base.OnInit();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>).FSM;
            _attackerCreateSystem = this.GetSystem<AttackerCreateSystem>();
        }

        protected override async UniTask<IAttacker> CreateAttackerInternal(string skillID, string attackerID)
        {
            var attacker = await GetOrCreateAttacker(skillID, attackerID);

            return attacker;
        }
    }
}
