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

public interface ISkillEffect<T> : IEffect where T : SkillEffectInfo
{
    T SkillEffectInfo { get; set; }
}

public abstract class SkillEffect<T> : ISkillEffect<T> where T : SkillEffectInfo
{
    public T SkillEffectInfo { get; set; }

    public SkillEffect(T skillEffectInfo)
    {
        SkillEffectInfo = skillEffectInfo;
    }

    public void Apply() => OnApply();
    public void Cancel() => OnCancel();

    protected abstract void OnApply();

    public abstract void OnCancel();
}
