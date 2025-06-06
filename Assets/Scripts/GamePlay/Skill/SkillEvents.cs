using System.Collections.Generic;
using Character;

public interface ISkillEvent
{
    ISkillContainer SkillsInSlot { get; set; }
    ISkillContainer SkillsInRelease { get; set; }
}

public abstract class SkillEvent : ISkillEvent
{
    public ISkillContainer SkillsInSlot { get; set; }
    public ISkillContainer SkillsInRelease { get; set; }

    public SkillEvent(ISkillContainer skillsInSlot, ISkillContainer skillsInRelease)
    {
        SkillsInSlot = skillsInSlot;
        SkillsInRelease = skillsInRelease;
    }
}

public class SkillSlotCountChangedEvent : SkillEvent
{
    public int Count { get; set; }

    public SkillSlotCountChangedEvent(int count, ISkillContainer skillsInSlot, ISkillContainer skillsInRelease) : base(skillsInSlot, skillsInRelease)
    {
        Count = count;
    }
}

public class SkillReleasedEvent : SkillEvent
{
    public ISkill Skill { get; set; }

    public SkillReleasedEvent(ISkill skill, ISkillContainer skillsInSlot, ISkillContainer skillsInRelease) : base(skillsInSlot, skillsInRelease)
    {
        Skill = skill;
    }
}

public class SkillAcquiredEvent : SkillEvent
{
    public ISkill Skill { get; set; }

    public SkillAcquiredEvent(ISkill skill, ISkillContainer skillsInSlot, ISkillContainer skillsInRelease) : base(skillsInSlot, skillsInRelease)
    {
        Skill = skill;
    }
}

public class SkillRemovedEvent : SkillEvent
{
    public string SkillID { get; set; }

    public SkillRemovedEvent(string skillID, ISkillContainer skillsInSlot, ISkillContainer skillsInRelease) : base(skillsInSlot, skillsInRelease)
    {
        SkillID = skillID;
    }
}
