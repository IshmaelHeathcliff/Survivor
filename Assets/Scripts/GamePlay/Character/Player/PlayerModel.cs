namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
        public BindableProperty<int> Coin { get; } = new BindableProperty<int>(0);
        public BindableProperty<int> Wood { get; } = new BindableProperty<int>(0);
    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
            AddModel("player", new PlayerModel());
        }

        public override PlayerModel Default()
        {
            return GetModel("player");
        }
    }
}