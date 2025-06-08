using Character.Damage;
using Character.Modifier;
using Character.Stat;
using Core;

namespace Character.Enemy
{
    public class EnemyController : CharacterControllerWithFSM<EnemyModel, EnemiesModel, EnemyStateID>
    {
        protected override void AddStates()
        {
            FSM.AddState(EnemyStateID.Idle, new EnemyIdleState(FSM, this));
            FSM.AddState(EnemyStateID.Patrol, new EnemyPatrolState(FSM, this));
            FSM.AddState(EnemyStateID.Attack, new EnemyAttackState(FSM, this));
            FSM.AddState(EnemyStateID.Chase, new EnemyChaseState(FSM, this));
            FSM.AddState(EnemyStateID.Hurt, new EnemyHurtState(FSM, this));
            FSM.AddState(EnemyStateID.Dead, new EnemyDeadState(FSM, this));
        }

        protected override void SetStats()
        {
            base.SetStats();
            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_increase", ID, 100);
            IStatModifier accuracyModifier = ModifierSystem.CreateStatModifier("accuracy_increase", ID, 100);
            healthModifier.Register();
            accuracyModifier.Register();
            Stats.Health.SetMaxValue();
        }

        protected override void MakeSureID()
        {
            ID = System.Guid.NewGuid().ToString();
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
        }

        void Start()
        {
            FSM.StartState(EnemyStateID.Idle);
        }


    }
}
