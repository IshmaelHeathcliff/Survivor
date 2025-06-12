using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Character.Damage;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Character.Player
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class PlayerAttacker : Attacker
    {
        [SerializeField] float _attackTime;
        [SerializeField] float _attackSpeed;
        [SerializeField] float _rotateSpeed;

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

            if (damageable == null || damageable.CompareTag("Player"))
            {
                return;
            }

            var keywords = new List<string>()
            {
                "Damage", "Attack", "Physical",
            };


            var damage = new AttackDamage(this, damageable, keywords, DamageType.Simple, Damage.BaseValue, 1, 1);
            damage.Apply();
        }

        protected override async UniTask Play()
        {
            transform.right = Direction;
            float leftTime = _attackTime;
            CancellationToken ct = GlobalCancellation.GetCombinedToken(this);

            while (leftTime > 0)
            {
                ct.ThrowIfCancellationRequested();
                transform.Translate((1 + _attackSpeed) * Time.fixedDeltaTime * Direction, Space.World);
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
