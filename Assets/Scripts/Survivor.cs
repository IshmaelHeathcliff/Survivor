using Character.Enemy;
using Character.State;
using Character.Modifier;
using Character.Player;
using SaveLoad;
using Scene;
using Character.Damage;

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
        RegisterSystem(new SkillReleaseSystem());
        RegisterSystem(new ResourceSystem());
        RegisterSystem(new CountSystem());
        RegisterSystem(new AttackerCreateSystem());

        RegisterUtility(new SaveLoadUtility());
    }
}