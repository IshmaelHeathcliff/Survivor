using GamePlay.Character.Player;
using GamePlay.Character.Stat;
using UnityEngine;

namespace UI
{
    public class PlayerHealthUIController : MonoBehaviour, IController
    {
        PlayerModel _model;

        void OnHealthChanged(float health, float maxHealth)
        {

        }

        void OnMaxHealthChanged(float maxHealth)
        {
        }

        void Start()
        {
            _model = this.GetModel<PlayersModel>().Current;
            var health = _model.Stats.GetStat("Health") as ConsumableStat;
            health?.RegisterWithInitValue(OnMaxHealthChanged).UnRegisterWhenDisabled(this);
            health?.RegisterWithInitValue(OnHealthChanged).UnRegisterWhenDisabled(this);
        }


        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
