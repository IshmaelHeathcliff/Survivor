using System.Collections.Generic;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Item;
using XYZRPGSystem.Gameplay.Skill;

namespace Gameplay.Character.Player
{
    public class PlayerModel : CharacterModel, IHasResources, IHasSkillPool
    {
        public IResourceContainer Resources { get; } = new ResourceContainer();
        public SkillPool SkillPool { get; set; } = new();
    }

    public class PlayersModel : CharactersModel<PlayerModel>
    {
        protected override void OnInit()
        {
            Current = AddModel("player");
        }
    }
}
