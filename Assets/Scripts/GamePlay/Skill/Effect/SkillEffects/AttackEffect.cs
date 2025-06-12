using System.Collections.Generic;
using GamePlay.Character.Damage;
using Cysharp.Threading.Tasks;
using Data.Config;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public class AttackEffect : SkillEffect<AttackEffectConfig>
    {
        readonly IAttackerController _attackerController;
        readonly List<IAttacker> _attackers = new();

        public AttackEffect(AttackEffectConfig config, IAttackerController attackerController) : base(config)
        {
            _attackerController = attackerController;
            Description = $"创建攻击器 {config.AttackerID}";
        }

        async UniTaskVoid CreateAttacker()
        {
            IEnumerable<IAttacker> attackers = await _attackerController.CreateAttackers(Owner.ID, SkillEffectConfig.AttackerID);
            if (Owner is not AttackSkill attackSkill)
            {
                Debug.LogError("AttackEffect is not owned by an AttackSkill");
                return;
            }
            foreach (IAttacker attacker in attackers)
            {
                attacker.SetSkill(attackSkill);
                _attackers.Add(attacker);
            }
        }


        protected override void OnApply()
        {
            CreateAttacker().Forget();
        }

        protected override void OnCancel()
        {
            foreach (IAttacker attacker in _attackers)
            {
                attacker?.Cancel();
            }

            _attackers.Clear();
        }
    }
}
