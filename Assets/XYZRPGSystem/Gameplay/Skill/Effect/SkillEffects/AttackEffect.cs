using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using XYZRPGSystem.Data.Config;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Damage.Attackers;
using XYZRPGSystem.Core;

namespace XYZRPGSystem.Gameplay.Skill.Effect
{
    public class AttackEffect : SkillEffect<AttackEffectConfig>
    {
        readonly List<IAttacker> _attackers = new();

        public AttackEffect(AttackEffectConfig config, ICharacterModel model) : base(config, model)
        {
            Description = $"创建攻击器 {config.AttackerID}";
        }

        async UniTaskVoid SetAttacker()
        {
            if (Owner is not AttackSkill attackSkill)
            {
                Debug.LogError("AttackEffect is not owned by an AttackSkill");
                return;
            }

            List<IAttacker> attackers = await Model.Controller.AttackerController.GetAttackers(Owner.ID, SkillEffectConfig.AttackerID);

            _attackers.AddRange(attackers);

            foreach (IAttacker attacker in attackers)
            {
                attacker.Attack().Forget();
            }

        }


        protected override void OnApply()
        {
            SetAttacker().Forget();
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
