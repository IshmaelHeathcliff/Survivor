using System;
using Character.Enemy;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        [SerializeField] float _hurtTime = 1f;

        FSM<EnemyStateID> _fsm;
        DropSystem _dropSystem;

        protected override void OnInit()
        {
            base.OnInit();
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>).FSM;
            _dropSystem = this.GetSystem<DropSystem>();
        }


        void Start()
        {
            SetStats(CharacterController.Stats);

            OnHurt.Register(() => Hurt().Forget()).UnRegisterWhenDisabled(this);
            OnDeath.Register(Dead).UnRegisterWhenDisabled(this);
        }

        async UniTaskVoid Hurt()
        {
            _fsm.ChangeState(EnemyStateID.Hurt);
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtTime));

            if (Health.CurrentValue <= 0)
            {
                OnDeath.Trigger();
                return;
            }

            _fsm.ChangeState(EnemyStateID.Idle);
        }


        void Dead()
        {
            this.GetSystem<CountSystem>().IncrementKillCount(1);

            string dropAddress = _dropSystem.GetDropAddress("coin");
            Addressables.InstantiateAsync(dropAddress, transform.position, Quaternion.identity);

            _fsm.ChangeState(EnemyStateID.Dead);


        }
    }
}
