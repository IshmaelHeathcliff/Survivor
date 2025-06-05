using Character.Damage;
using System;

public class AttackEffect : SkillEffect<AttackEffectConfig>
{
    readonly IAttackerController _attackerController;
    IAttacker _attacker;

    public AttackEffect(AttackEffectConfig skillEffectConfig, IAttackerController attackerController) : base(skillEffectConfig)
    {
        _attackerController = attackerController;
    }

    protected override async void OnApply()
    {
        try
        {
            _attacker = await _attackerController.CreateAttacker(SkillEffectConfig.AttackerAddress);
            _attacker.BaseDamage = SkillEffectConfig.Damage;
        }
        catch (OperationCanceledException)
        {
            _attacker.Cancel();
        }
    }

    public override void OnCancel()
    {
        _attacker.Cancel();
    }
}
