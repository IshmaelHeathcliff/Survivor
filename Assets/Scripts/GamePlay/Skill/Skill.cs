using System.Collections.Generic;

public interface ISkill
{
    string ID { get; set; }
    void OnEnable();
    void OnDisable();
    void Use();
    void Cancel();
}

public interface ISkill<T> : ISkill where T : SkillInfo
{
    T SkillInfo { get; set; }
}

public abstract class Skill<T> : ISkill<T> where T : SkillInfo
{
    public T SkillInfo { get; set; }
    public List<string> Keywords { get; set; }
    public string ID { get; set; }

    readonly List<IEffect> _skillEffectsOnEnable = new(); // 技能启用时生效的效果，比如Buff，需要关闭技能时主动 Cancel

    public Skill(T skillInfo, IEnumerable<IEffect> skillEffectsOnEnable)
    {
        ID = skillInfo.ID;
        SkillInfo = skillInfo;
        _skillEffectsOnEnable.AddRange(skillEffectsOnEnable);
    }

    public virtual void OnEnable()
    {
        foreach (IEffect skillEffect in _skillEffectsOnEnable)
        {
            skillEffect.Apply();
        }
    }

    public virtual void OnDisable()
    {
        foreach (IEffect skillEffect in _skillEffectsOnEnable)
        {
            skillEffect.Cancel();
        }

    }

    public abstract void Use();

    public abstract void Cancel();
}

public class ActiveSkill : Skill<ActiveSkillInfo>
{
    readonly List<IEffect> _skillEffectsOnUpdate = new(); // 每次使用技能时生效的效果，比如攻击，一般不需要主动 Cancel

    public float Cooldown => SkillInfo.Cooldown;
    public bool IsReady => _leftTime <= 0;

    float _leftTime;

    public ActiveSkill(ActiveSkillInfo skillInfo, IEnumerable<IEffect> skillEffectsOnEnable, IEnumerable<IEffect> skillEffectsOnUpdate) :
        base(skillInfo, skillEffectsOnEnable)
    {
        _skillEffectsOnUpdate.AddRange(skillEffectsOnUpdate);

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

        foreach (IEffect skillEffect in _skillEffectsOnUpdate)
        {
            skillEffect.Apply();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Cancel();
    }

    public override void Cancel()
    {
        foreach (IEffect skillEffect in _skillEffectsOnUpdate)
        {
            skillEffect.Cancel();
        }
    }
}

public class PassiveSkill : Skill<PassiveSkillInfo>
{
    public PassiveSkill(PassiveSkillInfo skillInfo, IEnumerable<IEffect> skillEffectsOnEnable) :
        base(skillInfo, skillEffectsOnEnable)
    {
    }

    public override void Cancel()
    {
        OnDisable();
    }

    public override void Use()
    {
        OnEnable();
    }
}
