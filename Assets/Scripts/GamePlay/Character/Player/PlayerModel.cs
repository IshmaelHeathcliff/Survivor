using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
        public Dictionary<string, BindableProperty<int>> Resources { get; } = new()
        {
            { "Coin", new BindableProperty<int>(0) },
            { "Wood", new BindableProperty<int>(0) },
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
