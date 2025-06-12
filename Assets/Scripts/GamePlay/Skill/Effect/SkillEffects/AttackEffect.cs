using Character.Damage;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Skill.Effect
{
    public class AttackEffect : SkillEffect<AttackEffectConfig>
    {
        readonly IAttackerController _attackerController;
        IAttacker _attacker;

        public AttackEffect(AttackEffectConfig skillEffectConfig, IAttackerController attackerController) : base(skillEffectConfig)
        {
            _attackerController = attackerController;
        }

        async UniTaskVoid CreateAttacker()
        {
            _attacker = await _attackerController.CreateAttacker(Owner.ID, SkillEffectConfig.AttackerID);
            if (Owner is not AttackSkill attackSkill)
            {
                Debug.LogError("AttackEffect is not owned by an AttackSkill");
                return;
            }
            _attacker.SetSkill(attackSkill);
        }


        protected override void OnApply()
        {
            CreateAttacker().Forget();
        }

        protected override void OnCancel()
        {
            _attacker?.Cancel();
        }
    }
}
