using GamePlay.Character.State;

namespace GamePlay.Character.Player
{
    public class PlayerStateController : StateController
    {
        protected override void SetStateContainer()
        {
            StateContainer = this.GetModel<PlayersModel>().Current.StateContainer;
        }
    }
}
