using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Stat;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using GamePlay.Character.Enemy;
using System.Linq;

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
        [SerializeField] float _randomDirectionFactor = 0.5f;

        public IStat ProjectileSpeed => AttackSkill.ProjectileSpeed;
        public IStat ChainCount => AttackSkill.ChainCount;
        public IStat PenetrateCount => AttackSkill.PenetrateCount;
        public IStat SplitCount => AttackSkill.SplitCount;
        public bool CanReturn => AttackSkill.CanReturn;
        public bool IsTargetLocked => AttackSkill.IsTargetLocked;


        Collider2D _collider;
        SpriteRenderer _renderer;
        readonly List<string> _damaged = new();
        int _penetrateLeft;
        int _chainLeft;
        bool _isTargetLocked;
        bool _isReturning;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }


        void Start()
        {
            _penetrateLeft = (int)PenetrateCount.Value;
            _chainLeft = (int)ChainCount.Value;
            _isTargetLocked = IsTargetLocked;
            Attack().Forget();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Damageable damageable = other.GetComponent<Damageable>();

            if (damageable == null
                || !damageable.CompareTag(TargetTag)
                || _damaged.Contains(damageable.ID)) // 不能对同一个敌人造成多次伤害
            {
                return;
            }

            ApplyDamage(damageable).Forget();
        }

        async UniTaskVoid ApplyDamage(IDamageable damageable)
        {
            _damaged.Add(damageable.ID);
            var damage = new AttackDamage(this, damageable, Keywords, DamageType.Simple, Damage.BaseValue, 1, 1);
            damage.Apply();
            _collider.enabled = false;

            if (_isReturning)
            {
                return;
            }

            // 如果目标未锁定，且有穿透次数，则穿透
            if (!_isTargetLocked && _penetrateLeft > 0)
            {
                _penetrateLeft--;
                _collider.enabled = true;
                return;
            }

            if (_chainLeft > 0)
            {
                _chainLeft--;
                if (Chain())
                {
                    _collider.enabled = true;
                    return;
                }
            }

            if (SplitCount.Value > 0)
            {
                Split();
            }

            if (CanReturn)
            {
                await Return();
            }

            Cancel().Forget();
        }

        void Split()
        {
            return;
        }

        bool Chain()
        {
            Target = this.GetSystem<PositionQuerySystem>().QueryClosest(TargetTag, transform.position, _damaged);
            if (Target == null)
            {
                Debug.LogError("Chain failed");
                return false;
            }

            Direction = (Target.position - transform.position).normalized;
            transform.right = Direction;
            return true;
        }

        async UniTask Return()
        {
            _isReturning = true;
            Target = AttackerController.CharacterController.CharacterModel.Transform;
            Direction = Target.position - transform.position;
            transform.right = Direction;
            _isTargetLocked = true;
            _collider.enabled = true;

            while (Vector2.SqrMagnitude(Target.position - transform.position) > 0.1f)
            {
                Move();
                await UniTask.WaitForFixedUpdate(GlobalCancellation.GetCombinedToken(this));
            }
        }

        void Move()
        {
            if (_isTargetLocked && Target != null)
            {
                Direction = ((Vector2)(Target.position - transform.position)).normalized;
            }

            transform.Translate(ProjectileSpeed.Value * Time.fixedDeltaTime * Direction, Space.World);
            transform.Rotate(0, 0, _rotateSpeed * 360 * Time.fixedDeltaTime);
        }

        protected override async UniTask Play()
        {
            if (Target != null)
            {
                Direction = Target.position - transform.position;
            }

            // 方向产生一定随机性
            float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Direction = (Direction + _randomDirectionFactor * Direction.magnitude * randomDirection).normalized;
            transform.right = Direction;

            float leftTime = Duration.Value;
            CancellationToken ct = GlobalCancellation.GetCombinedToken(this);

            while (!_isReturning && leftTime > 0)
            {
                ct.ThrowIfCancellationRequested();
                Move();
                leftTime -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(ct);
            }
        }

        public override async UniTaskVoid Attack()
        {
            try
            {
                await Play();

                if (CanReturn && !_isReturning)
                {
                    await Return();
                }

                if (_isReturning)
                {
                    while (Vector2.SqrMagnitude(Target.position - transform.position) > 0.1f)
                    {
                        await UniTask.WaitForFixedUpdate(GlobalCancellation.GetCombinedToken(this));
                    }
                }

                Cancel().Forget();
            }
            catch (OperationCanceledException)
            {

            }

        }

        public override async UniTaskVoid Cancel()
        {
            AttackerController?.RemoveAttacker(this);
            if (this != null)
            {
                Addressables.ReleaseInstance(gameObject);
            }

            await UniTask.CompletedTask;
        }
    }
}
