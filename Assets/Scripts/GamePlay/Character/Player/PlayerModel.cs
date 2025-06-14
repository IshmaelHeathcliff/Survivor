using System.Collections.Generic;
using GamePlay.Item;

namespace GamePlay.Character.Player
{
    public class PlayerModel : CharacterModel, IHasResources
    {
        public IResourceContainer Resources { get; } = new ResourceContainer();
    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
            Current = AddModel("player");
        }
    }
}
