using Character.Player;

namespace Character.State
{
    public class PlayerStateController : StateController
    {
        protected override void SetStateContainer()
        {
            StateContainer = this.GetModel<PlayersModel>().Current().StateContainer;
        }
    }
}
