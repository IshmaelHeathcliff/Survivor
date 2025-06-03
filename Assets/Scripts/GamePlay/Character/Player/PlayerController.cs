using Character.Damage;
using Character.Modifier;
using Character.Player;
using UnityEngine;

namespace Character
{
    public class PlayerController : MyCharacterController
    {
        [SerializeField] Vector3 _initialPosition;

        public void Respawn()
        {
            Model.Position = _initialPosition;
            Stats.Health.SetMaxValue();
            Stats.Mana.SetMaxValue();
        }

        protected override void SetStats()
        {
            base.SetStats();

            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_increase", "player", 100);
            IStatModifier manaModifier = ModifierSystem.CreateStatModifier("mana_increase", "player", 100);
            IStatModifier accuracyModifier = ModifierSystem.CreateStatModifier("accuracy_increase", "player", 100);

            healthModifier.Register();
            manaModifier.Register();
            accuracyModifier.Register();

            Stats.Health.SetMaxValue();
            Stats.Mana.SetMaxValue();
        }

        protected override void MakeSureModel()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = "player";
            }

            var playersModel = this.GetModel<PlayersModel>();
            if (playersModel.TryGetModel(ID, out PlayerModel model))
            {
                Model = model;
            }
            else
            {
                model = new PlayerModel(MoveController.Transform);
                playersModel.AddModel(ID, model);
                Model = model;
            }
        }

        protected override void OnInit()
        {
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
        }

        protected override void Awake()
        {
            base.Awake();
        }


    }
}
