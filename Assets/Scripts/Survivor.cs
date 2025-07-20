using Core.Scene;
using Gameplay.Skill;
using Gameplay.Item;
using Gameplay.Character.Enemy;
using Gameplay.Character.Player;

using XYZRPGSystem.Core;
using XYZRPGSystem.Gameplay.Status;
using XYZRPGSystem.Gameplay.Modifier;
using XYZRPGSystem.Gameplay.Damage.Attackers;
using XYZRPGSystem.Gameplay.Item;
using XYZRPGSystem.Gameplay.Skill;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Data.SaveLoad;
using XYZRPGSystem.Gameplay;

public class GameFrame : Architecture<GameFrame>
{
    protected override void Init()
    {
        RegisterModel(new PlayersModel());
        RegisterModel(new EnemiesModel());
        RegisterModel(new SceneModel());

        RegisterSystem(new SkillGachaSystem());
        RegisterSystem(new DropSystem());
        RegisterSystem(new EnemySystem());

        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new StatusCreateSystem());
        RegisterSystem(new SkillSystem());
        RegisterSystem(new SkillReleaseSystem());
        RegisterSystem(new ResourceSystem());
        RegisterSystem(new CountSystem());
        RegisterSystem(new AttackerSystem());
        RegisterSystem(new PositionQuerySystem());

        RegisterUtility(new DataPersistUtility());
    }
}
