using System.Collections.Generic;

namespace Skill.Effect
{
    public enum SkillEffectType
    {
        Attack, Summon, State, Stat, Move, System
    }

    public interface IEffect
    {
        void Enable(); // 激活效果
        void Apply(); // 应用效果，可在技能激活下反复应用
        void Cancel(); // 取消效果，取消激活状态下应用的所有效果
        void Disable(); // 禁用效果

        string Description { get; }

        ISkill Owner { get; set; }
    }

    public interface IEffect<T>
    {
        void Apply(T value);
        void Cancel(T value);

        ISkill Owner { get; set; }
    }

    public interface ISkillEffect<out T> : IEffect where T : SkillEffectConfig
    {
        T SkillEffectConfig { get; }
    }

    public abstract class SkillEffect<T> : ISkillEffect<T> where T : SkillEffectConfig
    {
        public T SkillEffectConfig { get; }
        public ISkill Owner { get; set; }
        public string Description { get; protected set; }


        protected SkillEffect(T skillEffectConfig)
        {
            SkillEffectConfig = skillEffectConfig;
            Description = skillEffectConfig.Description;
        }

        public virtual void Enable() => OnEnable();
        public virtual void Apply() => OnApply();
        public virtual void Cancel() => OnCancel();
        public virtual void Disable() => OnDisable();


        protected virtual void OnEnable()
        {
        }

        protected abstract void OnApply();
        protected abstract void OnCancel();

        protected virtual void OnDisable()
        {
        }

    }

    public class NestedSkillEffect<T> : SkillEffect<T> where T : NestedEffectConfig
    {
        public List<IEffect> ChildEffects { get; set; } = new();

        public NestedSkillEffect(T skillEffectConfig, IEnumerable<IEffect> childEffects) : base(skillEffectConfig)
        {
            ChildEffects.AddRange(childEffects);
        }

        protected override void OnEnable()
        {
            foreach (IEffect childEffect in ChildEffects)
            {
                childEffect.Enable();
            }
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

        protected override void OnDisable()
        {
            foreach (IEffect childEffect in ChildEffects)
            {
                childEffect.Disable();
            }
        }
    }
}
