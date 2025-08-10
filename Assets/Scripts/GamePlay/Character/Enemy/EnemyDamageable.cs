using Gameplay.Character.Player;
using UnityEngine;
using XYZRPGSystem.Core;
using XYZRPGSystem.Gameplay;
using XYZRPGSystem.Gameplay.Damage;
using XYZRPGSystem.Gameplay.Item;

namespace Gameplay.Character.Enemy
{
    public class EnemyDamageable : Damageable
    {
        FSM<EnemyStateID> _fsm;
        Collider2D _collider;

        protected override void OnInit()
        {
            base.OnInit();
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>)?.FSM;
            _collider = GetComponent<Collider2D>();
        }


        void Start()
        {
            SetStats(CharacterController.CharacterStats);

            OnHurt.Register(Hurt).UnRegisterWhenDisabled(this);
            OnDeath.Register(Dead).UnRegisterWhenDisabled(this);
        }

        public override void TakeDamage(float damage)
        {
            if (!IsDamageable)
            {
                // TODO 关注此处逻辑可能出现时的问题
                // Debug.Log("Can't take damage", this);
                return;
            }

            Health.ChangeCurrentValue(-damage);
            // Debug.Log($"TakeDamage: {damage}, Left Health: {Health.CurrentValue}");
            _fsm.ChangeState(EnemyStateID.Hurt);
            OnHurt.Trigger();
        }

        void Hurt()
        {
        }


        void Dead()
        {
            PlayerModel playerModel = this.GetModel<PlayersModel>().Current;
            this.GetSystem<CountSystem>().IncrementKillCount(playerModel, 1);

            ResourceSystem resourceSystem = this.GetSystem<ResourceSystem>();

            // 玩家击杀奖励
            resourceSystem.AcquireResource("Coin", (int)playerModel.Stats.GetStat("CoinOnKill").Value, playerModel);
            resourceSystem.AcquireResource("Wood", (int)playerModel.Stats.GetStat("WoodOnKill").Value, playerModel);

            // 敌人死亡奖励
            resourceSystem.AcquireResource("Coin", (int)CharacterController.CharacterStats.GetStat("CoinOnDead").Value, playerModel);
            resourceSystem.AcquireResource("Wood", (int)CharacterController.CharacterStats.GetStat("WoodOnDead").Value, playerModel);
        }
    }
}
