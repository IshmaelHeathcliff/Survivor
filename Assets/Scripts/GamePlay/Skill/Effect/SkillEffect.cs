using Character.Damage;
using Unity.VisualScripting;

public enum SkillEffectType
{
    Attack, Summon, State, Stat, Move, System
}

public interface IEffect
{
    void Apply();
    void Cancel();
}

public interface ISkillEffect<T> : IEffect where T : SkillEffectConfig
{
    T SkillEffectConfig { get; set; }
}

public abstract class SkillEffect<T> : ISkillEffect<T> where T : SkillEffectConfig
{
    public T SkillEffectConfig { get; set; }

    public SkillEffect(T skillEffectConfig)
    {
        SkillEffectConfig = skillEffectConfig;
    }

    public void Apply() => OnApply();
    public void Cancel() => OnCancel();

    protected abstract void OnApply();

    public abstract void OnCancel();
}
