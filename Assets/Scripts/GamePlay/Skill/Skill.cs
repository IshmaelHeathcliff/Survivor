using System.Collections.Generic;
using Character.Stat;
using UnityEngine;

public interface ISkill
{
    string ID { get; set; }
    string Name { get; }
    SkillStats SkillStats { get; }
    void Enable();
    void Disable();
    void Use();
    void Cancel();
    void SetEffects(IEnumerable<IEffect> effectsOnEnable, IEnumerable<IEffect> effectsOnUpdate);
    string Description { get; }
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
    public string ID { get; set; }
    public string Name => SkillConfig.Name;
    public virtual List<string> Keywords => SkillConfig.Keywords;
    public virtual string Description => SkillConfig.Description;

    // 技能启用时生效的效果，比如Buff，需要关闭技能时主动 Cancel
    protected readonly List<IEffect> SkillEffectsOnEnable = new();
    // 每次使用技能时生效的效果，比如攻击，一般不需要主动 Cancel
    protected readonly List<IEffect> SkillEffectsOnUpdate = new();

    public Skill(T skillConfig, CharacterStats characterStats)
    {
        ID = skillConfig.ID;
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
            SkillEffectsOnUpdate.AddRange(effectsOnUpdate);
        }
    }

    public virtual void Enable()
    {
        foreach (IEffect skillEffect in SkillEffectsOnEnable)
        {
            skillEffect.Apply();
        }
    }

    public virtual void Disable()
    {
        foreach (IEffect skillEffect in SkillEffectsOnEnable)
        {
            skillEffect.Cancel();
        }

    }

    public abstract void Use();

    public abstract void Cancel();
}

public class RepetitiveSkill : Skill<RepetitiveSkillConfig>
{
    public IStat CooldownInverse => SkillStats.GetStat("CooldownInverse");
    public float Cooldown => 1f / CooldownInverse.Value;
    public bool IsReady => _leftTime <= 0;

    float _leftTime;

    public RepetitiveSkill(RepetitiveSkillConfig skillConfig, CharacterStats characterStats, IEnumerable<IEffect> skillEffectsOnEnable = null, IEnumerable<IEffect> skillEffectsOnUpdate = null) :
        base(skillConfig, characterStats)
    {
        SetEffects(skillEffectsOnEnable, skillEffectsOnUpdate);
        CooldownInverse.BaseValue = 1f / skillConfig.Cooldown;
        _leftTime = 0;
    }

    public void Update(float deltaTime)
    {
        if (!IsReady)
        {
            _leftTime -= deltaTime;
        }
    }

    public override void Use()
    {
        if (!IsReady)
        {
            return;
        }

        _leftTime += Cooldown;

        foreach (IEffect skillEffect in SkillEffectsOnUpdate)
        {
            skillEffect.Apply();
        }
    }

    public override void Disable()
    {
        base.Disable();
        Cancel();
    }

    public override void Cancel()
    {
        foreach (IEffect skillEffect in SkillEffectsOnUpdate)
        {
            skillEffect.Cancel();
        }
    }
}

public class OneTimeSkill : Skill<OneTimeSkillConfig>
{
    public OneTimeSkill(OneTimeSkillConfig skillConfig, CharacterStats characterStats, IEnumerable<IEffect> skillEffectsOnEnable = null) :
        base(skillConfig, characterStats)
    {
        SetEffects(skillEffectsOnEnable, null);
    }

    public override void Cancel()
    {
        Disable();
    }

    public override void Use()
    {
        Enable();
    }
}


// 可以创建Attacker的Skill
public class AttackSkill : RepetitiveSkill
{
    public IStat Damage => SkillStats.GetStat("Damage");
    public IStat CriticalChance => SkillStats.GetStat("CriticalChance");
    public IStat CriticalMultiplier => SkillStats.GetStat("CriticalMultiplier");
    public IStat AttackArea => SkillStats.GetStat("AttackArea");
    public IStat Duration => SkillStats.GetStat("Duration");

    public AttackSkill(AttackSkillConfig skillConfig, CharacterStats characterStats, IEnumerable<IEffect> skillEffectsOnEnable = null, IEnumerable<IEffect> skillEffectsOnUpdate = null)
        : base(skillConfig, characterStats, skillEffectsOnEnable, skillEffectsOnUpdate)
    {
        Damage.BaseValue = skillConfig.Damage;
        CriticalChance.BaseValue = skillConfig.CriticalChance;
        CriticalMultiplier.BaseValue = skillConfig.CriticalMultiplier;
        AttackArea.BaseValue = skillConfig.AttackArea;
        Duration.BaseValue = skillConfig.Duration;
    }
}
