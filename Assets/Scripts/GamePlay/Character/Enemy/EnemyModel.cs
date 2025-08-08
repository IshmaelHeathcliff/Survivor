using System.Linq;
using XYZRPGSystem.Gameplay.Character;

namespace Gameplay.Character.Enemy
{
    public class EnemyModel : CharacterModel
    {
        public string EnemyID { get; set; }
    }

    public class EnemiesModel : CharactersModel<EnemyModel>
    {
        public int GetCountByEnemyID(string enemyID)
        {
            return Models.Values.Count(model => model.EnemyID == enemyID);
        }

        protected override void OnInit()
        {
        }
    }
}
