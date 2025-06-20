using System.Collections.Generic;
using System.Linq;
using GamePlay.Character;

namespace GamePlay.Skill
{
    public interface IReleaseEvent
    {
        ICharacterModel Model { get; set; }
    }


    /// <summary>
    /// 技能释放条件接口
    /// </summary>
    public interface ISkillReleaseCondition
    {
        EasyEvent<IReleaseEvent> OnRelease { get; }
        List<string> SkillsToRelease { get; }
        void CheckCondition(IReleaseEvent e);
        bool IsMet(ICharacterModel model);
        string Description { get; }
    }

    public abstract class SkillReleaseCondition : ISkillReleaseCondition
    {
        public EasyEvent<IReleaseEvent> OnRelease { get; set; } = new();
        public string Description { get; protected set; }
        public List<string> SkillsToRelease { get; set; }

        public SkillReleaseCondition(List<string> skillsToRelease, string description)
        {
            Description = description;
            SkillsToRelease = skillsToRelease ?? new List<string>();
        }

        public virtual void CheckCondition(IReleaseEvent e)
        {
            if (IsMet(e.Model))
            {
                OnRelease.Trigger(e);
            }
        }

        public abstract bool IsMet(ICharacterModel model);
    }


    public abstract class ReleaseOnSkillAcquiredCondition : SkillReleaseCondition
    {
        public List<string> RequiredSkillIDs { get; set; }
        public ReleaseOnSkillAcquiredCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, string description) : base(skillsToRelease, description)
        {
            RequiredSkillIDs = requiredSkillIDs ?? new List<string>();
        }
    }

    /// <summary>
    /// 指定技能组合释放条件
    /// </summary>
    public class SpecificSkillsReleaseCondition : ReleaseOnSkillAcquiredCondition
    {
        public SpecificSkillsReleaseCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, string description) : base(requiredSkillIDs, skillsToRelease, description)
        {
            Description = string.IsNullOrEmpty(description) ? $"凑齐技能: {string.Join(", ", RequiredSkillIDs)}" : description;
        }

        public override bool IsMet(ICharacterModel model)
        {
            return model.HasSkills(RequiredSkillIDs);
        }
    }

    public class AnySkillsCountReleaseCondition : ReleaseOnSkillAcquiredCondition
    {
        public int RequiredCount { get; set; }
        public AnySkillsCountReleaseCondition(List<string> requiredSkillIDs, List<string> skillsToRelease, int requiredCount, string description) : base(requiredSkillIDs, skillsToRelease, description)
        {
            RequiredCount = requiredCount;
            Description = string.IsNullOrEmpty(description) ? $"凑齐技能 {string.Join(", ", RequiredSkillIDs)} 至少 {RequiredCount} 个" : description;
        }

        public override bool IsMet(ICharacterModel model)
        {
            return model.SkillsInSlot.HasCount(RequiredSkillIDs) >= RequiredCount;
        }
    }

    public class ValueCountCondition : SkillReleaseCondition
    {
        public string ValueID { get; set; }
        public int Value { get; set; }

        public ValueCountCondition(string valueID, int value, List<string> skillsToRelease, string description) : base(skillsToRelease, description)
        {
            ValueID = valueID;
            Value = value;
        }

        public override bool IsMet(ICharacterModel model)
        {
            return model.CountValues[ValueID].Value >= Value;
        }
    }

    /// <summary>
    /// 复合释放条件（AND逻辑）
    /// </summary>
    public class CompositeAndReleaseCondition : SkillReleaseCondition
    {
        public List<ISkillReleaseCondition> Conditions { get; set; }
        public CompositeAndReleaseCondition(List<ISkillReleaseCondition> conditions, string description = "") : base(conditions.SelectMany(c => c.SkillsToRelease).ToList(), description)
        {
            Conditions = conditions ?? new List<ISkillReleaseCondition>();
            Description = string.IsNullOrEmpty(description)
                ? $"满足所有条件: {string.Join(" 且 ", Conditions.Select(c => c.Description))}"
                : description;
        }

        public override bool IsMet(ICharacterModel model)
        {
            return Conditions.Count > 0 && Conditions.All(c => c.IsMet(model));
        }
    }

    /// <summary>
    /// 复合释放条件（OR逻辑）
    /// </summary>
    public class CompositeOrReleaseCondition : SkillReleaseCondition
    {
        public List<ISkillReleaseCondition> Conditions { get; set; }
        public CompositeOrReleaseCondition(List<ISkillReleaseCondition> conditions, string description = "") : base(conditions.SelectMany(c => c.SkillsToRelease).ToList(), description)
        {
            Conditions = conditions ?? new List<ISkillReleaseCondition>();
            Description = string.IsNullOrEmpty(description)
                ? $"满足任一条件: {string.Join(" 或 ", Conditions.Select(c => c.Description))}"
                : description;
        }

        public override bool IsMet(ICharacterModel model)
        {
            return Conditions.Count > 0 && Conditions.Any(c => c.IsMet(model));
        }
    }
}
