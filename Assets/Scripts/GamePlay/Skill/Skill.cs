using System.Collections.Generic;

public interface ISkill
{
    void OnEnable();
    void OnDisable();
    void Use();
}

public interface ISkill<T> : ISkill where T : SkillInfo
{
    T SkillInfo { get; set; }
}

public abstract class Skill<T> : ISkill<T> where T : SkillInfo
{
    public T SkillInfo { get; set; }
    public List<string> Keywords { get; set; }

    readonly List<IEffect> _skillEffectsOnUpdate = new(); // 每次使用技能时生效的效果，比如攻击，一般不需要主动 Cancel
    readonly List<IEffect> _skillEffectsOnEnable = new(); // 技能启用时生效的效果，比如Buff，需要关闭技能时主动 Cancel

    public Skill(T skillInfo, IEnumerable<IEffect> skillEffectsOnUpdate, IEnumerable<IEffect> skillEffectsOnEnable)
    {
        SkillInfo = skillInfo;
        _skillEffectsOnUpdate.AddRange(skillEffectsOnUpdate);
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
        foreach (IEffect skillEffect in _skillEffectsOnUpdate)
        {
            skillEffect.Cancel();
        }

        foreach (IEffect skillEffect in _skillEffectsOnEnable)
        {
            skillEffect.Cancel();
        }

    }

    public virtual void Use()
    {
        foreach (IEffect skillEffect in _skillEffectsOnUpdate)
        {
            skillEffect.Apply();
        }
    }


}

public class ActiveSkill : Skill<ActiveSkillInfo>
{
    public float Cooldown => SkillInfo.Cooldown;
    public bool IsReady => _leftTime <= 0;

    float _leftTime;

    public ActiveSkill(ActiveSkillInfo skillInfo, IEnumerable<IEffect> skillEffectsOnUpdate, IEnumerable<IEffect> skillEffectsOnEnable) :
        base(skillInfo, skillEffectsOnUpdate, skillEffectsOnEnable)
    {
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
        base.Use();
    }
}

public class PassiveSkill : Skill<PassiveSkillInfo>
{
    public PassiveSkill(PassiveSkillInfo skillInfo, IEnumerable<IEffect> skillEffectsOnUpdate, IEnumerable<IEffect> skillEffectsOnEnable) :
        base(skillInfo, skillEffectsOnUpdate, skillEffectsOnEnable)
    {
    }
}
