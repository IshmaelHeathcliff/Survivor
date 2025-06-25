using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Player;
using GamePlay.Character.Stat;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public class PlayerHealthUIController : MonoBehaviour, IController
    {
        [SerializeField] AssetReferenceGameObject _healthUIReference;

        PlayerModel _model;

        readonly List<HealthUI> _healthUIList = new();

        async UniTask SetMaxHealth(int maxHealth)
        {
            int heartCount = maxHealth / HealthUI.HeartSize;
            int leftHealth = maxHealth % HealthUI.HeartSize;

            if (leftHealth > 0)
            {
                heartCount++;
            }

            if (_healthUIList.Count < heartCount)
            {
                for (int i = _healthUIList.Count; i < heartCount; i++)
                {
                    GameObject obj = await Addressables.InstantiateAsync(_healthUIReference, transform);
                    _healthUIList.Add(obj.GetComponent<HealthUI>());
                }
            }

            for (int i = 0; i < _healthUIList.Count; i++)
            {
                if (maxHealth >= HealthUI.HeartSize)
                {
                    _healthUIList[i].SetMaxHealth(HealthUI.HeartSize);
                    maxHealth -= HealthUI.HeartSize;
                }
                else
                {
                    _healthUIList[i].SetMaxHealth(maxHealth);
                    maxHealth = 0;
                }
            }
        }

        void SetCurrentHealth(int health)
        {
            for (int i = 0; i < _healthUIList.Count; i++)
            {
                if (health >= HealthUI.HeartSize)
                {
                    _healthUIList[i].SetCurrentHealth(HealthUI.HeartSize);
                    health -= HealthUI.HeartSize;
                }
                else
                {
                    _healthUIList[i].SetCurrentHealth(health);
                    health = 0;
                }
            }
        }

        async UniTaskVoid SetHealth(float health, float maxHealth)
        {
            await SetMaxHealth((int)maxHealth);
            SetCurrentHealth((int)health);
        }

        void OnHealthChanged(float health, float maxHealth)
        {
            SetHealth(health, maxHealth).Forget();
        }

        void OnMaxHealthChanged(float maxHealth)
        {
            SetMaxHealth((int)maxHealth).Forget();
        }

        void Start()
        {
            _model = this.GetModel<PlayersModel>().Current;
            var health = _model.Stats.GetStat("Health") as ConsumableStat;
            health?.Register(OnMaxHealthChanged).UnRegisterWhenDisabled(this);
            health?.RegisterWithInitValue(OnHealthChanged).UnRegisterWhenDisabled(this);
        }


        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
