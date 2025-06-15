using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GamePlay.Character.Damage
{
    public class SelfAttacker : Attacker
    {
        Collider2D _collider;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        void Start()
        {
            // _collider.enabled = false;

        }

        protected override UniTask Play()
        {
            // _collider.enabled = true;
            // TODO: 添加攻击特效
            return UniTask.CompletedTask;
        }

        public override async UniTaskVoid Attack()
        {
            await Play();
        }


        public override void Cancel()
        {
            return;
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (!AttackerController.CanAttack)
            {
                return;
            }

            Damageable damageable = other.GetComponent<Damageable>();

            if (damageable == null || !damageable.CompareTag(TargetTag))
            {
                return;
            }


            var damage = new AttackDamage(this, damageable, Keywords, DamageType.Simple, Damage.BaseValue, 1, 1);
            damage.Apply();
        }

    }
}
