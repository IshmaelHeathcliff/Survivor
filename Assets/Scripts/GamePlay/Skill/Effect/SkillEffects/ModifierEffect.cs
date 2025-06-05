using Character.Modifier;

public class ModifierEffect : SkillEffect<ModifierEffectConfig>
{
    readonly IStatModifier _modifier;

    public ModifierEffect(ModifierEffectConfig config, ModifierSystem modifierSystem, IStatModifierFactory factory) : base(config)
    {
        _modifier = modifierSystem.CreateStatModifier(config.ModifierID, factory, config.Value);
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
