using GamePlay.Character.Player;
using GamePlay.Stat;
using GamePlay.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerResourceUIController : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _coinText;
        [SerializeField] TextMeshProUGUI _woodText;

        PlayerModel _model;


        void OnValidate()
        {

            if (_coinText == null)
            {
                _coinText = transform.Find("Coin").GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_woodText == null)
            {
                _woodText = transform.Find("Wood").GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        void Awake()
        {
        }

        void Start()
        {
            _model = this.GetModel<PlayersModel>().Current;
            var health = _model.Stats.GetStat("Health") as ConsumableStat;

            this.GetSystem<ResourceSystem>().RegisterWithInitValue(ResourceType.Coin, OnCoinChanged, _model).UnRegisterWhenDisabled(this);
            this.GetSystem<ResourceSystem>().RegisterWithInitValue(ResourceType.Wood, OnWoodChanged, _model).UnRegisterWhenDisabled(this);
        }

        void OnCoinChanged(int coin)
        {
            _coinText.text = coin.ToString();
        }

        void OnWoodChanged(int wood)
        {
            _woodText.text = wood.ToString();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
