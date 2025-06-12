using System.Collections.Generic;

namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
        public Dictionary<string, BindableProperty<int>> Resources { get; } = new()
        {
            { "Coin", new BindableProperty<int>() },
            { "Wood", new BindableProperty<int>() },
        };

    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
            Current = AddModel("player");
        }
    }
}
