using Character.Enemy;
using Character.State;
using Character.Modifier;
using Character.Player;
using SaveLoad;
using Scene;

public class PixelRPG : Architecture<PixelRPG>
{
    protected override void Init()
    {
        RegisterModel(new PlayersModel());
        RegisterModel(new EnemiesModel());
        RegisterModel(new SceneModel());

        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new StateCreateSystem());

        RegisterUtility(new SaveLoadUtility());
    }
}