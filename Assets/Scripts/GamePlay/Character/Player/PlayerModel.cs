using UnityEngine;

namespace Character.Player
{
    public class PlayerModel : CharacterModel
    {
        public PlayerModel(Transform transform) : base(transform)
        {
        }

        public BindableProperty<int> Coin { get; } = new BindableProperty<int>(0);
        public BindableProperty<int> Wood { get; } = new BindableProperty<int>(0);
    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        public string CurrentID = "player";

        public override PlayerModel Current()
        {
            TryGetModel(CurrentID, out PlayerModel model);
            return model;
        }

        protected override void OnInit()
        {
        }
    }
}
