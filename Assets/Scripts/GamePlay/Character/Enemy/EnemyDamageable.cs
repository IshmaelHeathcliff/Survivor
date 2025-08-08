using Gameplay.Character.Player;

using XYZRPGSystem.Core;
using XYZRPGSystem.Gameplay;
using XYZRPGSystem.Gameplay.Damage;
using XYZRPGSystem.Gameplay.Item;

namespace Gameplay.Character.Enemy
{
    public class EnemyDamageable : Damageable
    {
        FSM<EnemyStateID> _fsm;

        protected override void OnInit()
        {
            base.OnInit();
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
            _fsm = (CharacterController as IHasFSM<EnemyStateID>)?.FSM;
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

            ResourceSystem _resourceSystem = this.GetSystem<ResourceSystem>();

            // 玩家击杀奖励
            _resourceSystem.AcquireResource("Coin", (int)playerModel.Stats.GetStat("CoinOnKill").Value, playerModel);
            _resourceSystem.AcquireResource("Wood", (int)playerModel.Stats.GetStat("WoodOnKill").Value, playerModel);

            // 敌人死亡奖励
            _resourceSystem.AcquireResource("Coin", (int)CharacterController.CharacterStats.GetStat("CoinOnDead").Value, playerModel);
            _resourceSystem.AcquireResource("Wood", (int)CharacterController.CharacterStats.GetStat("WoodOnDead").Value, playerModel);
        }
    }
}
