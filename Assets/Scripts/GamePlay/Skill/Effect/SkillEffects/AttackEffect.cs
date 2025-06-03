using Character.Damage;
using System;

public class AttackEffect : SkillEffect<AttackEffectInfo>
{
    readonly IAttackerController _attackerController;
    IAttacker _attacker;

    public AttackEffect(AttackEffectInfo skillEffectInfo, IAttackerController attackerController) : base(skillEffectInfo)
    {
        _attackerController = attackerController;
    }

    protected override async void OnApply()
    {
        try
        {
            _attacker = await _attackerController.CreateAttacker(SkillEffectInfo.AttackerAddress);
            _attacker.BaseDamage = SkillEffectInfo.Damage;
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
