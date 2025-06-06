using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using UnityEngine;

/// <summary>
/// 技能释放条件接口
/// </summary>
public interface ISkillReleaseCondition
{
    EasyEvent OnCondition { get; set; }
    void CheckCondition(ISkillEvent e);
    string GetDescription();
}

public abstract class ReleaseOnSkillAcquiredCondition : ISkillReleaseCondition
{
    public List<string> RequiredSkillIDs { get; set; }
    public List<string> SkillsToRelease { get; set; }
    public EasyEvent OnCondition { get; set; }
    public string Description { get; set; }
    public ReleaseOnSkillAcquiredCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, string description)
    {
        OnCondition = new EasyEvent();
        RequiredSkillIDs = requiredSkillIDs ?? new List<string>();
        SkillsToRelease = skillsToRelease ?? new List<string>();
        Description = description;
    }

    public virtual void CheckCondition(ISkillEvent e)
    {
        if (e is not SkillAcquiredEvent skillAcquiredEvent)
        {
            return;
        }

        if (!RequiredSkillIDs.Contains(skillAcquiredEvent.Skill.ID))
        {
            return;
        }
    }

    public abstract string GetDescription();
}

/// <summary>
/// 指定技能组合释放条件
/// </summary>
public class SpecificSkillsReleaseCondition : ReleaseOnSkillAcquiredCondition
{

    public SpecificSkillsReleaseCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, string description) : base(requiredSkillIDs, skillsToRelease, description)
    {
        RequiredSkillIDs = requiredSkillIDs ?? new List<string>();
        SkillsToRelease = skillsToRelease ?? new List<string>();
        Description = string.IsNullOrEmpty(description) ? $"凑齐技能: {string.Join(", ", RequiredSkillIDs)}" : description;
        OnCondition = new EasyEvent();
    }

    public override void CheckCondition(ISkillEvent e)
    {
        base.CheckCondition(e);

        if (e.SkillsInSlot.HasSkills(RequiredSkillIDs))
        {
            OnCondition.Trigger();
        }
    }

    public override string GetDescription()
    {
        return Description;
    }
}

public class AnySkillsCountReleaseCondition : ReleaseOnSkillAcquiredCondition
{
    public int RequiredCount { get; set; }
    public AnySkillsCountReleaseCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, int requiredCount, string description) : base(requiredSkillIDs, skillsToRelease, description)
    {
        RequiredSkillIDs = requiredSkillIDs ?? new List<string>();
        SkillsToRelease = skillsToRelease ?? new List<string>();
        RequiredCount = requiredCount;
        Description = string.IsNullOrEmpty(description) ? $"凑齐技能 {string.Join(", ", RequiredSkillIDs)} 至少 {RequiredCount} 个" : description;
        OnCondition = new EasyEvent();
    }

    public override void CheckCondition(ISkillEvent e)
    {
        base.CheckCondition(e);

        if (e.SkillsInSlot.HasCount(RequiredSkillIDs) >= RequiredCount)
        {
            OnCondition.Trigger();
        }
    }

    public override string GetDescription()
    {
        return Description;
    }
}

/// <summary>
/// 复合释放条件（AND逻辑）
/// </summary>
public class CompositeAndReleaseCondition : ISkillReleaseCondition
{
    public List<ISkillReleaseCondition> Conditions { get; set; }
    public string Description { get; set; }
    public EasyEvent OnCondition { get; set; }
    public CompositeAndReleaseCondition(List<ISkillReleaseCondition> conditions, string description = "")
    {
        Conditions = conditions ?? new List<ISkillReleaseCondition>();
        Description = string.IsNullOrEmpty(description)
            ? $"满足所有条件: {string.Join(" 且 ", Conditions.Select(c => c.GetDescription()))}"
            : description;
        OnCondition = new EasyEvent();

        // 用于记录每个子条件是否已触发
        bool[] triggeredFlags = new bool[Conditions.Count];

        for (int i = 0; i < Conditions.Count; i++)
        {
            int index = i; // 捕获当前索引
            Conditions[i].OnCondition.Register(() =>
            {
                triggeredFlags[index] = true;
                // 检查所有子条件是否都已触发
                if (triggeredFlags.All(flag => flag))
                {
                    OnCondition.Trigger();
                }
            });
        }
    }

    public void CheckCondition(ISkillEvent e)
    {
        foreach (ISkillReleaseCondition condition in Conditions)
        {
            condition.CheckCondition(e);
        }
    }

    public string GetDescription()
    {
        return Description;
    }
}

/// <summary>
/// 复合释放条件（OR逻辑）
/// </summary>
public class CompositeOrReleaseCondition : ISkillReleaseCondition
{
    public List<ISkillReleaseCondition> Conditions { get; set; }
    public string Description { get; set; }
    public EasyEvent OnCondition { get; set; }
    public CompositeOrReleaseCondition(List<ISkillReleaseCondition> conditions, string description = "")
    {
        Conditions = conditions ?? new List<ISkillReleaseCondition>();
        Description = string.IsNullOrEmpty(description)
            ? $"满足任一条件: {string.Join(" 或 ", Conditions.Select(c => c.GetDescription()))}"
            : description;
        OnCondition = new EasyEvent();

        foreach (ISkillReleaseCondition condition in Conditions)
        {
            condition.OnCondition.Register(OnCondition.Trigger);
        }
    }

    public void CheckCondition(ISkillEvent e)
    {
        foreach (ISkillReleaseCondition condition in Conditions)
        {
            condition.CheckCondition(e);
        }
    }

    public string GetDescription()
    {
        return Description;
    }
}
