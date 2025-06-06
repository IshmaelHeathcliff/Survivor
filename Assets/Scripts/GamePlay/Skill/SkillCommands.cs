using Character.Player;
using UnityEngine;

public abstract class PlayerSkillCommand : AbstractCommand
{
    protected ISkillContainer SkillsInSlot { get; private set; }
    protected ISkillContainer SkillsReleased { get; private set; }

    protected override void OnExecute()
    {
        PlayerModel model = this.GetModel<PlayersModel>().Current();
        SkillsInSlot = model.SkillsInSlot;
        SkillsReleased = model.SkillsReleased;

        ExecuteSkillCommand();
    }

    protected abstract void ExecuteSkillCommand();
}

public class SetSkillSlotCountCommand : PlayerSkillCommand
{
    readonly int _count;

    public SetSkillSlotCountCommand(int count) : base()
    {
        _count = count;
    }

    protected override void ExecuteSkillCommand()
    {
        SkillsInSlot.MaxCount = _count;
        this.SendEvent(new SkillSlotCountChangedEvent()
        {
            Count = _count,
        });
    }
}

public class AcquireSkillCommand : PlayerSkillCommand
{
    readonly ISkill _skill;

    public AcquireSkillCommand(ISkill skill) : base()
    {
        _skill = skill;
    }

    protected override void ExecuteSkillCommand()
    {
        if (SkillsInSlot.Count >= SkillsInSlot.MaxCount)
        {
            Debug.Log($"技能槽位已满，最大数量: {SkillsInSlot.MaxCount}");
            return;
        }

        if (!SkillsInSlot.AddSkill(_skill))
        {
            return;
        }

        this.SendEvent(new SkillAcquiredEvent()
        {
            Skill = _skill,
        });
    }
}

public class ReleaseSkillCommand : PlayerSkillCommand
{
    readonly ISkill _skill;

    public ReleaseSkillCommand(ISkill skill) : base()
    {
        _skill = skill;
    }

    protected override void ExecuteSkillCommand()
    {
        if (!SkillsInSlot.RemoveSkill(_skill.ID))
        {
            return;
        }

        if (!SkillsReleased.AddSkill(_skill))
        {
            return;
        }

        this.SendEvent(new SkillReleasedEvent()
        {
            Skill = _skill,
        });
    }
}

public class RemoveSkillCommand : PlayerSkillCommand
{
    readonly string _skillID;

    public RemoveSkillCommand(string skillID) : base()
    {
        _skillID = skillID;
    }

    protected override void ExecuteSkillCommand()
    {
        if (!SkillsReleased.RemoveSkill(_skillID))
        {
            return;
        }

        this.SendEvent(new SkillRemovedEvent()
        {
            SkillID = _skillID,
        });
    }
}
