using System.Collections.Generic;
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
    ISkill Owner { get; set; }
}

public interface IEffect<T>
{
    void Apply(T value);
    void Cancel(T value);
    ISkill Owner { get; set; }
}

public interface ISkillEffect<T> : IEffect where T : SkillEffectConfig
{
    T SkillEffectConfig { get; }
}

public abstract class SkillEffect<T> : ISkillEffect<T> where T : SkillEffectConfig
{
    public T SkillEffectConfig { get; }
    public ISkill Owner { get; set; }

    protected SkillEffect(T skillEffectConfig)
    {
        SkillEffectConfig = skillEffectConfig;
    }

    public virtual void Apply() => OnApply();
    public virtual void Cancel() => OnCancel();

    protected abstract void OnApply();

    protected abstract void OnCancel();
}

public class NestedSkillEffect<T> : SkillEffect<T> where T : NestedEffectConfig
{
    public List<IEffect> ChildEffects { get; set; } = new();

    public NestedSkillEffect(T skillEffectConfig, IEnumerable<IEffect> childEffects) : base(skillEffectConfig)
    {
        ChildEffects.AddRange(childEffects);
    }

    protected override void OnApply()
    {
        foreach (IEffect childEffect in ChildEffects)
        {
            childEffect.Apply();
        }

    }

    protected override void OnCancel()
    {
        foreach (IEffect childEffect in ChildEffects)
        {
            childEffect.Cancel();
        }
    }
}
