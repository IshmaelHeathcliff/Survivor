using System;
using Character.Stat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public interface IDamageable : ICharacterControlled
    {
        EasyEvent OnHurt { get; }
        EasyEvent OnDeath { get; }
        IConsumableStat Health { get; }
        bool IsDamageable { get; set; }
        void TakeDamage(float damage);

    }
    public abstract class Damageable : CharacterControlled, IDamageable, IController
    {
        [SerializeField] float _invincibleTime;

        public EasyEvent OnHurt { get; protected set; }
        public EasyEvent OnDeath { get; protected set; }
        public IConsumableStat Health { get; protected set; }
        public bool IsDamageable { get; set; } = true;

        protected override void OnInit()
        {
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
            // Debug.Log("Left Health:" + Health.CurrentValue);
            OnHurt.Trigger();

            if (Health.CurrentValue <= 0)
            {
                OnDeath.Trigger();
            }

            Invincible().Forget();
        }

        async UniTaskVoid Invincible()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_invincibleTime), cancellationToken: GlobalCancellation.GetCombinedToken(this));
            IsDamageable = true;
        }


        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
