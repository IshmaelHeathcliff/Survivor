using System;
using System.Threading;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using System.Linq;
using XYZRPGSystem.Gameplay.Skill;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Character.Enemy
{
    public class EnemyAttackState : EnemyState
    {
        public EnemyAttackState(FSM<EnemyStateID> fsm, EnemyController target) : base(fsm, target)
        {
        }

        EnemyAttackerController AttackerController { get; set; }
        CancellationTokenSource _cts = new();
        ISkill _attackSkill;

        protected override bool OnCondition()
        {
            return FSM.CurrentStateId is EnemyStateID.Chase;
        }

        protected async override void OnEnter()
        {
            AttackerController = Target.AttackerController as EnemyAttackerController;

            AttackerController.CanAttack = true;

            var attackSkills = Target.CharacterModel.SkillsInSlot.GetAllSkills().Where(x => x is AttackSkill).ToList();

            _cts = new();
            try
            {
                // 随机选择一个技能并使用
                if (attackSkills.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, attackSkills.Count);
                    _attackSkill = attackSkills[randomIndex];
                    _attackSkill.Use();
                }
                else
                {
                    Debug.Log($"[EnemyAttackState] 没有可用的攻击技能");
                }

                UniTask animationTask = MoveController.PlayAnimation(EnemyMoveController.Attack);
                await UniTask.WhenAll(animationTask);
                FSM.ChangeState(EnemyStateID.Idle);
            }
            catch (OperationCanceledException)
            {
            }
        }

        protected override void OnExit()
        {
            AttackerController.CanAttack = false;
            _attackSkill?.Cancel();
            _attackSkill = null;
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
