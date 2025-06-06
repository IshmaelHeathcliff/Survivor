using System.Collections.Generic;
using UnityEngine;

namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
        public PlayerModel(string id, Transform transform) : base(id, transform)
        {
        }

        public BindableProperty<int> Coin { get; } = new BindableProperty<int>(0);
        public BindableProperty<int> Wood { get; } = new BindableProperty<int>(0);

    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
        }
    }
}
