using Gameplay.Skill;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;

namespace Gameplay.Character.Enemy
{
    public class EnemyController : CharacterControllerWithFSM<EnemyModel, EnemiesModel, EnemyStateID>
    {
        [SerializeField] string _skillPoolPath;

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

        protected override void Start()
        {
            base.Start();
            FSM.StartState(EnemyStateID.Idle);

            this.GetSystem<SkillGachaSystem>().InitSkillPool(Model, _skillPoolPath);
        }


    }
}
