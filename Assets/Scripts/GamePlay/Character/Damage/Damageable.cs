using System;
using Core;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Stat;
using UnityEngine;

namespace GamePlay.Character.Damage
{
    public interface IDamageable : ICharacterControlled
    {
        string ID { get; }
        EasyEvent OnHurt { get; }
        EasyEvent OnDeath { get; }
        Transform Transform { get; }
        IConsumableStat Health { get; }
        bool IsDamageable { get; set; }
        void TakeDamage(float damage);

    }
    public abstract class Damageable : CharacterControlled, IDamageable, IController
    {
        [SerializeField] float _invincibleTime;

        public string ID => CharacterController.CharacterModel.ID;
        public EasyEvent OnHurt { get; protected set; }
        public EasyEvent OnDeath { get; protected set; }
        public Transform Transform { get; protected set; }
        public IConsumableStat Health { get; protected set; }
        public bool IsDamageable { get; set; } = true;

        protected override void OnInit()
        {
            Transform = transform;
        }

        protected override void OnDeinit()
        {
        }

        public void SetStats(Stats stats)
        {
            Health = stats.GetStat("Health") as IConsumableStat;
        }

        public virtual void TakeDamage(float damage)
        {
            if (!IsDamageable)
            {
                return;
            }

            IsDamageable = false;

            Health.ChangeCurrentValue(-damage);
            // Debug.Log($"TakeDamage: {damage}, Left Health: {Health.CurrentValue}");
            OnHurt.Trigger();

            if (Health.CurrentValue <= 0)
            {
                OnDeath.Trigger();
            }

            Invincible().Forget();
        }

        async UniTaskVoid Invincible()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_invincibleTime), cancellationToken: GlobalCancellation.GetCombinedTokenSource(this).Token);
            IsDamageable = true;
        }


        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
