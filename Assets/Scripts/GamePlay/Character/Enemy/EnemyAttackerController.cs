using System.Collections.Generic;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using XYZRPGSystem.Gameplay.Damage.Attackers;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Damage;
using UnityEngine;

namespace Gameplay.Character.Enemy
{
    public class EnemyAttackerController : AttackerController
    {
        FSM<EnemyStateID> _fsm;
        public FSM<EnemyStateID> FSM => _fsm;

        AttackerSystem _attackerSystem;

        protected override void OnInit()
        {
            base.OnInit();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>)?.FSM;
            _attackerSystem = this.GetSystem<AttackerSystem>();
        }

        public override async UniTask<List<IAttacker>> GetAttackers(string skillID, string attackerID)
        {

            List<IAttacker> attackers = await base.GetAttackers(skillID, attackerID);

            if (attackerID == "self")
            {
                return attackers;
            }

            // 向最近敌人位置发射
            List<string> selected = new();
            foreach (IAttacker attacker in attackers)
            {
                attacker.Target = this.GetSystem<PositionQuerySystem>().QueryClosest(TargetTag, transform.position, selected);
                if (attacker.Target != null)
                {
                    selected.Add(attacker.Target.GetComponentInChildren<Damageable>().ID);
                }
                else
                {
                    Debug.Log($"[EnemyAttackerController] 未找到攻击器目标");
                }
            }

            AttackerParent.DetachChildren();
            return attackers;
        }
    }
}
