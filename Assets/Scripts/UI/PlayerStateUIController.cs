using UI;

namespace GamePlay.Character.Player
{
    public class PlayerStateUIController : StateUIController
    {
        protected override void SetStateContainer()
        {
            StateContainer = this.GetModel<PlayersModel>().Current.StateContainer;
        }
    }
}
