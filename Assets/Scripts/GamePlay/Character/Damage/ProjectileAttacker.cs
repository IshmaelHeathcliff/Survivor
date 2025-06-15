using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Stat;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GamePlay.Character.Damage
{
    public interface IProjectileAttacker : IAttacker
    {
        IStat ProjectileSpeed { get; }
    }

    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class ProjectileAttacker : Attacker, IProjectileAttacker
    {
        [SerializeField] float _rotateSpeed;

        public IStat ProjectileSpeed => AttackSkill.ProjectileSpeed;

        Collider2D _collider;
        SpriteRenderer _renderer;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }


        void Start()
        {
            Attack().Forget();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Damageable damageable = other.GetComponent<Damageable>();

            if (damageable == null || !damageable.CompareTag(TargetTag))
            {
                return;
            }

            ApplyDamage(damageable).Forget();
        }

        async UniTaskVoid ApplyDamage(IDamageable damageable)
        {
            var damage = new AttackDamage(this, damageable, Keywords, DamageType.Simple, Damage.BaseValue, 1, 1);
            damage.Apply();
            _collider.enabled = false;
            await UniTask.Delay(100);
            Cancel();
        }

        protected override async UniTask Play()
        {
            transform.right = Direction;
            float leftTime = Duration.Value;
            CancellationToken ct = GlobalCancellation.GetCombinedToken(this);

            while (leftTime > 0)
            {
                ct.ThrowIfCancellationRequested();
                transform.Translate(ProjectileSpeed.Value * Time.fixedDeltaTime * Direction, Space.World);
                transform.Rotate(0, 0, _rotateSpeed * 360 * Time.fixedDeltaTime);
                leftTime -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(ct);
            }
        }

        public override async UniTaskVoid Attack()
        {
            try
            {
                await Play();
                Cancel();
            }
            catch (OperationCanceledException)
            {

            }

        }

        public override void Cancel()
        {
            AttackerController?.RemoveAttacker(this);
            if (this != null)
            {
                Addressables.ReleaseInstance(gameObject);
            }
        }
    }
}
