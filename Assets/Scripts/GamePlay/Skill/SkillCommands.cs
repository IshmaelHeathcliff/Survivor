using Character;
using Character.Damage;
using UnityEngine;

public class SetSkillEnvCommand : AbstractCommand
{
    readonly IAttackerController _attackerController;
    readonly ICharacterModel _model;

    public SetSkillEnvCommand(IAttackerController attackerController, ICharacterModel model) : base()
    {
        _attackerController = attackerController;
        _model = model;
    }

    protected override void OnExecute()
    {
        this.GetSystem<SkillSystem>().SetEnv(_attackerController, _model);
    }
}

public class SetSkillSlotCountCommand : AbstractCommand
{
    readonly int _count;

    public SetSkillSlotCountCommand(int count) : base()
    {
        _count = count;
    }

    protected override void OnExecute()
    {
        this.GetSystem<SkillSystem>().SetSkillSlotCount(_count);
    }
}

public class AcquireSkillCommand : AbstractCommand
{
    readonly string _skillID;

    public AcquireSkillCommand(string skillID) : base()
    {
        _skillID = skillID;
    }

    protected override void OnExecute()
    {
        this.GetSystem<SkillSystem>().AcquireSkill(_skillID);
    }
}

public class ReleaseSkillCommand : AbstractCommand
{
    readonly string _skillID;

    public ReleaseSkillCommand(string skillID) : base()
    {
        _skillID = skillID;
    }

    protected override void OnExecute()
    {
        this.GetSystem<SkillSystem>().ReleaseSkill(_skillID);
    }
}

public class RemoveSkillCommand : AbstractCommand
{
    readonly string _skillID;

    public RemoveSkillCommand(string skillID) : base()
    {
        _skillID = skillID;
    }

    protected override void OnExecute()
    {
        this.GetSystem<SkillSystem>().RemoveSkill(_skillID);
    }
}
