using System;
using Core;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Damage;
using GamePlay.Character.Player;
using GamePlay.Item;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GamePlay.Character.Enemy
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
            _fsm = (CharacterController as IHasFSM<EnemyStateID>)?.FSM;
            _dropSystem = this.GetSystem<DropSystem>();
        }


        void Start()
        {
            SetStats(CharacterController.CharaterStats);

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
            var playerModel = this.GetModel<PlayersModel>().Current;
            this.GetSystem<CountSystem>().IncrementKillCount(playerModel, 1);

            this.GetSystem<ResourceSystem>().AcquireResource("Coin", (int)playerModel.Stats.GetStat("CoinOnKill").Value, playerModel);
            this.GetSystem<ResourceSystem>().AcquireResource("Wood", (int)playerModel.Stats.GetStat("WoodOnKill").Value, playerModel);

            _fsm.ChangeState(EnemyStateID.Dead);
        }
    }
}
