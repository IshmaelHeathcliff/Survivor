using GamePlay.Character.Player;
using GamePlay.Character.Stat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIController : MonoBehaviour, IController
    {
        [SerializeField] Slider _healthSlider;
        [SerializeField] TextMeshProUGUI _coinText;
        [SerializeField] TextMeshProUGUI _woodText;

        PlayerModel _model;

        void OnHealthChanged(float health, float maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
            _healthSlider.value = health;
        }

        void OnMaxHealthChanged(float maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
        }


        void OnValidate()
        {
            if (_healthSlider == null)
            {
                _healthSlider = transform.Find("Health").GetComponent<Slider>();
            }

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

            health?.RegisterWithInitValue(OnMaxHealthChanged).UnRegisterWhenDisabled(this);

            health?.RegisterWithInitValue(OnHealthChanged).UnRegisterWhenDisabled(this);

            _model.Resources["Coin"].RegisterWithInitValue(OnCoinChanged).UnRegisterWhenDisabled(this);
            _model.Resources["Wood"].RegisterWithInitValue(OnWoodChanged).UnRegisterWhenDisabled(this);
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
