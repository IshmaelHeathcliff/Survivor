using Character.Modifier;

public class ModifierEffect : SkillEffect<ModifierEffectInfo>
{
    readonly IStatModifier _modifier;

    public ModifierEffect(ModifierEffectInfo info, ModifierSystem modifierSystem, IStatModifierFactory factory) : base(info)
    {
        _modifier = modifierSystem.CreateStatModifier(info.ModifierID, factory, info.Value);
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
