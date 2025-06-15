using System.Collections.Generic;
using Data.Config;
using GamePlay.Character.Stat;
using GamePlay.Skill.Effect;

namespace GamePlay.Skill
{
    public enum SkillRarity
    {
        Common,
        Magic,
        Rare,
        Epic,
        Legendary,
    }
    public enum SkillType
    {
        OneTime,
        Repetitive,
        Attack
    }

    public interface ISkill
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        SkillRarity Rarity { get; }
        SkillStats SkillStats { get; }
        void Enable();
        void Disable();
        void Use();
        void Cancel();
        void SetEffects(IEnumerable<IEffect> effectsOnEnable, IEnumerable<IEffect> effectsOnUpdate);
        List<string> Keywords { get; }
    }

    public interface ISkill<T> : ISkill where T : SkillConfig
    {
        T SkillConfig { get; set; }
    }

    public abstract class Skill<T> : ISkill<T> where T : SkillConfig
    {
        public T SkillConfig { get; set; }
        public SkillStats SkillStats { get; }
        public string ID => SkillConfig.ID;
        public string Name => SkillConfig.Name;

        //
        public virtual List<string> Keywords => SkillConfig.Keywords;
        public virtual string Description => SkillConfig.Description;
        public SkillRarity Rarity => SkillConfig.Rarity;

        // 技能启用时生效的效果，比如Buff，需要关闭技能时主动 Cancel
        protected readonly List<IEffect> SkillEffectsOnEnable = new();
        // 每次使用技能时生效的效果，比如攻击，一般不需要主动 Cancel
        protected readonly List<IEffect> SkillEffectsOnUse = new();

        public Skill(T skillConfig, CharacterStats characterStats)
        {
            SkillConfig = skillConfig;
            SkillStats = new SkillStats(skillConfig.Keywords, characterStats);
        }

        public virtual void SetEffects(IEnumerable<IEffect> effectsOnEnable, IEnumerable<IEffect> effectsOnUpdate)
        {
            if (effectsOnEnable != null)
            {
                SkillEffectsOnEnable.AddRange(effectsOnEnable);
            }

            if (effectsOnUpdate != null)
            {
                SkillEffectsOnUse.AddRange(effectsOnUpdate);
            }
        }

        public virtual void Enable()
        {
            foreach (IEffect skillEffect in SkillEffectsOnEnable)
            {
                skillEffect.Enable();
                skillEffect.Apply();
                // Debug.Log(skillEffect.Description);
            }
        }

        public abstract void Use();

        public abstract void Cancel();

        public virtual void Disable()
        {
            foreach (IEffect skillEffect in SkillEffectsOnEnable)
            {
                skillEffect.Cancel();
                skillEffect.Disable();
            }
        }
    }

    public class RepetitiveSkill : Skill<RepetitiveSkillConfig>
    {
        public IKeywordStat CooldownInverse => SkillStats.GetKeywordStat("CooldownInverse");
        public float Cooldown => 1f / CooldownInverse.Value;
        public bool IsReady => _leftTime <= 0;

        bool _isEnabled;
        float _leftTime;

        public RepetitiveSkill(RepetitiveSkillConfig skillConfig, CharacterStats characterStats) :
            base(skillConfig, characterStats)
        {
            CooldownInverse.BaseValue = 1f / skillConfig.Cooldown;
            _leftTime = 0;
        }

        public override void Enable()
        {
            base.Enable();
            _isEnabled = true;
            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Enable();
            }
        }

        public void Update(float deltaTime)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!IsReady)
            {
                _leftTime -= deltaTime;
            }
        }

        public override void Use()
        {
            if (!_isEnabled || !IsReady)
            {
                return;
            }

            _leftTime += Cooldown;

            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Apply();
            }
        }

        public override void Cancel()
        {
            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Cancel();
            }
        }

        public override void Disable()
        {
            base.Disable();
            _isEnabled = false;

            Cancel();
            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Disable();
            }
        }
    }

    public class OneTimeSkill : Skill<OneTimeSkillConfig>
    {
        public OneTimeSkill(OneTimeSkillConfig skillConfig, CharacterStats characterStats) :
            base(skillConfig, characterStats)
        {
        }

        public override void Enable()
        {
            base.Enable();
            Use();
        }

        public override void Use()
        {
            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Enable();
                skillEffect.Apply();
                // Debug.Log(skillEffect.Description);
            }
        }

        public override void Cancel()
        {
            foreach (IEffect skillEffect in SkillEffectsOnUse)
            {
                skillEffect.Cancel();
                skillEffect.Disable();
            }
        }

        public override void Disable()
        {
            Cancel();
            base.Disable();
        }
    }
}

