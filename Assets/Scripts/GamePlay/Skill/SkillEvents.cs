using Character;

namespace Skill
{
    public interface ISkillEvent
    {
        ISkillContainer SkillsInSlot { get; set; }
        ISkillContainer SkillsInRelease { get; set; }
    }

    public abstract class SkillEvent : ISkillEvent
    {
        public ICharacterModel Model { get; set; }
        public ISkillContainer SkillsInSlot { get; set; }
        public ISkillContainer SkillsInRelease { get; set; }

        public SkillEvent(ICharacterModel model)
        {
            Model = model;
            SkillsInSlot = model.SkillsInSlot;
            SkillsInRelease = model.SkillsReleased;
        }
    }

    public class SkillSlotCountChangedEvent : SkillEvent
    {
        public int Count { get; set; }

        public SkillSlotCountChangedEvent(int count, ICharacterModel model) : base(model)
        {
            Count = count;
        }
    }

    public class SkillReleasedEvent : SkillEvent
    {
        public ISkill Skill { get; set; }

        public SkillReleasedEvent(ISkill skill, ICharacterModel model) : base(model)
        {
            Skill = skill;
        }
    }

    public class SkillAcquiredEvent : SkillEvent, IReleaseEvent
    {
        public ISkill Skill { get; set; }

        public SkillAcquiredEvent(ISkill skill, ICharacterModel model) : base(model)
        {
            Skill = skill;
        }
    }

    public class SkillRemovedEvent : SkillEvent
    {
        public string SkillID { get; set; }

        public SkillRemovedEvent(string skillID, ICharacterModel model) : base(model)
        {
            SkillID = skillID;
        }
    }
}
