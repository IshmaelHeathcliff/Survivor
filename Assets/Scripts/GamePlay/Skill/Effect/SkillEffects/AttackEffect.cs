using Character.Damage;
using System;
using Cysharp.Threading.Tasks;

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
        _attacker = await _attackerController.CreateAttacker(SkillEffectConfig.AttackerAddress);
        _attacker.BaseDamage = SkillEffectConfig.Damage;
    }

    protected override void OnApply()
    {
        CreateAttacker().Forget();
    }

    public override void OnCancel()
    {
        _attacker.Cancel();
    }
}
