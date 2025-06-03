using Character.Damage;
using Character.Modifier;
using Character.Stat;
using Core;

namespace Character.Enemy
{
    public class EnemyController : CharacterControllerWithFSM<EnemyStateId>
    {
        protected override void AddStates()
        {
            FSM.AddState(EnemyStateId.Idle, new EnemyIdleState(FSM, this));
            FSM.AddState(EnemyStateId.Patrol, new EnemyPatrolState(FSM, this));
            FSM.AddState(EnemyStateId.Attack, new EnemyAttackState(FSM, this));
            FSM.AddState(EnemyStateId.Chase, new EnemyChaseState(FSM, this));
            FSM.AddState(EnemyStateId.Hurt, new EnemyHurtState(FSM, this));
            FSM.AddState(EnemyStateId.Dead, new EnemyDeadState(FSM, this));
        }

        protected override void SetStats()
        {
            Model.Stats.Health.BaseValue = 100;
            Model.Stats.Accuracy.BaseValue = 100;
            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_increase", ID, 100);
            IStatModifier accuracyModifier = ModifierSystem.CreateStatModifier("accuracy_increase", ID, 100);
            healthModifier.Register();
            accuracyModifier.Register();
            Stats.Health.SetMaxValue();
        }

        protected override void MakeSureModel()
        {
            ID = System.Guid.NewGuid().ToString();
            EnemiesModel enemiesModel = this.GetModel<EnemiesModel>();
            if (enemiesModel.TryGetModel(ID, out EnemyModel model))
            {
                Model = model;
            }
            else
            {
                model = new EnemyModel(MoveController.Transform);
                enemiesModel.AddModel(ID, model);
                Model = model;
            }
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
            FSM.StartState(EnemyStateId.Idle);
        }


    }
}
