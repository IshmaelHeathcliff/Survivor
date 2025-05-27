using Character.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class PlayerUIController : MonoBehaviour, IController
    {
        [SerializeField] Slider _healthSlider;
        [SerializeField] Slider _manaSlider;
        [SerializeField] TextMeshProUGUI _coinText;
        [SerializeField] TextMeshProUGUI _woodText;

        PlayerModel _model;

        void OnHealthChanged(float health, float maxHealth)
        {
            _healthSlider.value = health;
            _healthSlider.maxValue = maxHealth;
        }

        void OnMaxHealthChanged(float maxHealth)
        {
            _healthSlider.maxValue = maxHealth;
        }

        void OnManaChanged(float mana, float maxMana)
        {
            _manaSlider.value = mana;
            _manaSlider.maxValue = maxMana;
        }

        void OnMaxManaChanged(float maxMana)
        {
            _manaSlider.maxValue = maxMana;
        }

        void OnValidate()
        {
            if (_healthSlider == null)
            {
                _healthSlider = transform.Find("Health").GetComponent<Slider>();
            }

            if (_manaSlider == null)
            {
                _manaSlider = transform.Find("Mana").GetComponent<Slider>();
            }

            if (_coinText == null)
            {
                _coinText = transform.Find("Coin").GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_woodText == null)
            {
                _woodText = transform.Find("Wood").GetComponent<TextMeshProUGUI>();
            }
        }

        void Awake()
        {
            _model = this.GetModel<PlayersModel>().Default();
        }

        void OnEnable()
        {
            Stat.ConsumableStat health = _model.Stats.Health;
            Stat.ConsumableStat mana = _model.Stats.Mana;
            health.Register(OnMaxHealthChanged).UnRegisterWhenDisabled(this);
            mana.Register(OnMaxManaChanged).UnRegisterWhenDisabled(this);
            health.RegisterWithInitValue(OnHealthChanged).UnRegisterWhenDisabled(this);
            mana.RegisterWithInitValue(OnManaChanged).UnRegisterWhenDisabled(this);
            _model.Coin.RegisterWithInitValue(OnCoinChanged).UnRegisterWhenDisabled(this);
            _model.Wood.RegisterWithInitValue(OnWoodChanged).UnRegisterWhenDisabled(this);
        }

        void OnCoinChanged(int coin)
        {
            _coinText.text = coin.ToString();
        }

        void OnWoodChanged(int wood)
        {
            _woodText.text = wood.ToString();
        }

        void Start()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
