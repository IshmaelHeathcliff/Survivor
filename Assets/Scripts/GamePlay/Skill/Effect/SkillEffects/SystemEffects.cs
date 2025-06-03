using Character.Modifier;

public class SystemEffect : SkillEffect<SystemEffectInfo>
{
    AbstractSystem _system;

    public SystemEffect(SystemEffectInfo info, AbstractSystem system) : base(info)
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
