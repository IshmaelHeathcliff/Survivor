using System.Collections.Generic;
using GamePlay.Character;
using Cysharp.Threading.Tasks;
using Data.Config;
using UnityEngine;
using GamePlay.Damage.Attackers;

namespace GamePlay.Skill.Effect
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

            foreach (IAttacker attacker in attackers)
            {
                attacker.SetSkill(attackSkill);
                _attackers.Add(attacker);
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
                attacker?.Cancel().Forget();
            }

            _attackers.Clear();
        }
    }
}
