using UnityEngine;

namespace Character.Enemy
{
    public class EnemyModel : CharacterModel
    {
        public EnemyModel(Transform transform) : base(transform)
        {
        }
    }

    public class EnemiesModel : CharactersModel<EnemyModel>
    {
        public override EnemyModel Current()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInit()
        {
        }
    }
}
