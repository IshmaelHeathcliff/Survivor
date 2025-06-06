using System.Collections.Generic;
using Character;

public struct SkillSlotCountChangedEvent
{
    public int Count;
}

public struct SkillReleasedEvent
{
    public ISkill Skill;
}

public struct SkillAcquiredEvent
{
    public ISkill Skill;
}

public struct SkillRemovedEvent
{
    public string SkillID;
}