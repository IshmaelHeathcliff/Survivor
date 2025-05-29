﻿using Character.Damage;
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
            transform.position = _initialPosition;
            Model.Stats.Health.SetMaxValue();
        }

        protected override void SetStats()
        {
            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_base", "player", 100);
            IStatModifier manaModifier = ModifierSystem.CreateStatModifier("mana_base", "player", 100);
            IStatModifier accuracyModifier = ModifierSystem.CreateStatModifier("accuracy_base", "player", 100);
            healthModifier.Register();
            manaModifier.Register();
            accuracyModifier.Register();
            Stats.Health.SetMaxValue();
            Stats.Mana.SetMaxValue();
        }

        protected override void OnInit()
        {
            if (ID == null || ID == "")
            {
                ID = "player";
            }

            Model = this.GetModel<PlayersModel>().AddModel(ID, new PlayerModel());
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
        }
    }
}
