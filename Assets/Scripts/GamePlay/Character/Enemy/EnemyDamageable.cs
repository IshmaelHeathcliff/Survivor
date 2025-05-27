using System;
using Character.Enemy;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public class EnemyDamageable : Damageable
    {
        [SerializeField] float _hurtTime = 1f;

        FSM<EnemyStateId> _fsm;
        DropSystem _dropSystem;

        protected override void OnInit()
        {
            base.OnInit();
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
            _fsm = (CharacterController as IHasFSM<EnemyStateId>).FSM;
            _dropSystem = GameFrame.Interface.GetSystem<DropSystem>();
        }


        void Start()
        {
            SetStats(CharacterController.Stats);

            OnHurt.Register(() => Hurt().Forget()).UnRegisterWhenDisabled(this);
            OnDeath.Register(Dead).UnRegisterWhenDisabled(this);
        }

        public override void TakeDamage(float damage)
        {
            if (!IsDamageable)
            {
                return;
            }

            Health.ChangeCurrentValue(-damage);
            // Debug.Log("Left Health:" + Health.CurrentValue);
            OnHurt.Trigger();
        }

        async UniTaskVoid Hurt()
        {
            _fsm.ChangeState(EnemyStateId.Hurt);
            await UniTask.Delay(TimeSpan.FromSeconds(_hurtTime));

            if (Health.CurrentValue <= 0)
            {
                OnDeath.Trigger();
                return;
            }

            _fsm.ChangeState(EnemyStateId.Idle);
        }


        void Dead()
        {
            _fsm.ChangeState(EnemyStateId.Dead);
            GameObject drop = _dropSystem.GetDrop("coin");
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}
