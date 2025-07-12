using Core;
using Core.Scene;
using GamePlay.Character.Enemy;
using GamePlay.State;
using GamePlay.Modifier;
using GamePlay.Character.Player;
using GamePlay.Damage.Attackers;
using Data.SaveLoad;
using GamePlay.Item;
using GamePlay.Skill;
using GamePlay.Character;

public class GameFrame : Architecture<GameFrame>
{
    protected override void Init()
    {
        RegisterModel(new PlayersModel());
        RegisterModel(new EnemiesModel());
        RegisterModel(new SceneModel());

        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new StateCreateSystem());
        RegisterSystem(new DropSystem());
        RegisterSystem(new SkillSystem());
        RegisterSystem(new SkillGachaSystem()); ;
        RegisterSystem(new SkillReleaseSystem());
        RegisterSystem(new ResourceSystem());
        RegisterSystem(new CountSystem());
        RegisterSystem(new AttackerSystem());
        RegisterSystem(new PositionQuerySystem());

        RegisterUtility(new SaveLoadUtility());
    }
}
