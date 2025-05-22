﻿using Character.Damage;
using Character.Modifier;
using Character.Stat;
using Core;
using UnityEditor;

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
            IStatModifier healthModifier = ModifierSystem.CreateStatModifier("health_base", ID, 100);
            IStatModifier accuracyModifier = ModifierSystem.CreateStatModifier("accuracy_base", ID, 100);
            healthModifier.Register();
            accuracyModifier.Register();
            Stats.Health.SetMaxValue();
        }

        protected override void OnInit()
        {
            base.OnInit();
            ID ??= GUID.Generate().ToString();
            Model = this.GetModel<EnemiesModel>().GetModel(ID);
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
