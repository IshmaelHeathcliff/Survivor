using Character.Player;

namespace Character.State
{
    public class PlayerStateController : StateController
    {
        protected override void Awake()
        {
            StateContainer = this.GetModel<PlayersModel>().Current().StateContainer;
            base.Awake();
        }
    }
}
