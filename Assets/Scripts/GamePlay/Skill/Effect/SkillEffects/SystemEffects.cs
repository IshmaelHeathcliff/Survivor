using Character.Modifier;

public class SystemEffect : SkillEffect<SystemEffectConfig>
{
    AbstractSystem _system;

    public SystemEffect(SystemEffectConfig config, AbstractSystem system) : base(config)
    {
        _system = system;
    }

    public override void OnCancel()
    {
    }

    protected override void OnApply()
    {
    }
}
