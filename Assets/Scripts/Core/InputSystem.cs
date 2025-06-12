namespace Core
{
    public class InputSystem : AbstractSystem
    {
        PlayerInput _input;

        public PlayerInput.PlayerActions PlayerActionMap { get; private set; }


        protected override void OnInit()
        {
            _input = new PlayerInput();
            PlayerActionMap = _input.Player;
        }
    }
}
