using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class PlayerAttacker : Attacker
    {
        [SerializeField] float _animationTime;
        [SerializeField] float _animationSpeed;
        [SerializeField] float _animationRotationSpeed;

        Collider2D _collider;
        SpriteRenderer _renderer;

        public Vector2 Direction { get; set; }

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


            var damage = new AttackDamage(this, damageable, keywords, DamageType.Physical, 100, 1, 1);
            damage.Apply();
        }

        protected override async UniTask Play()
        {
            transform.right = Direction;
            float leftTime = _animationTime;
            while (leftTime > 0)
            {
                this.GetCancellationTokenOnDestroy().ThrowIfCancellationRequested();
                transform.Translate((1 + _animationSpeed) * Time.fixedDeltaTime * Direction, Space.World);
                transform.Rotate(0, 0, _animationRotationSpeed * 360 * Time.fixedDeltaTime);
                leftTime -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(this.GetCancellationTokenOnDestroy());
            }
        }

        public override async UniTaskVoid Attack()
        {
            try
            {
                await Play();
                AttackerController.RemoveAttacker(this);
                Destroy(gameObject);
            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}
