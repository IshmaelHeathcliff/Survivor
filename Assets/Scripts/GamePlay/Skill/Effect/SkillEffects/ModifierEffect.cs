using Character.Modifier;

public class ModifierEffect : SkillEffect<ModifierEffectInfo>
{
    readonly IStatModifier _modifier;

    public ModifierEffect(ModifierEffectInfo skillEffectInfo, IStatModifier modifier) : base(skillEffectInfo)
    {
        _modifier = modifier;
    }

    public override void OnCancel()
    {
        _modifier.Unregister();
    }

    protected override void OnApply()
    {
        _modifier.Register();
    }
}
