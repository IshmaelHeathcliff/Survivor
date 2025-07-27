using System;
using System.Threading;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Item;
using XYZRPGSystem.Gameplay.Stat;
using XYZRPGSystem.Gameplay.Skill;

namespace XYZRPGSystem.Gameplay.Damage.Attackers
{
    public interface IProjectileAttacker : IAttacker
    {
        IStat ProjectileSpeed { get; }
    }

    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class ProjectileAttacker : Attacker, IProjectileAttacker
    {
        [SerializeField] float _collisionRadius = 1f;
        [SerializeField] float _rotateSpeed;
        [SerializeField] float _randomDirectionFactor = 0.5f;

        ProjectileAttackSkill ProjectileAttackSkill => AttackSkill as ProjectileAttackSkill;

        public IStat ProjectileSpeed => ProjectileAttackSkill.ProjectileSpeed;
        public IStat ChainCount => ProjectileAttackSkill.ChainCount;
        public IStat PenetrateCount => ProjectileAttackSkill.PenetrateCount;
        public IStat SplitCount => ProjectileAttackSkill.SplitCount;
        public bool CanReturn => ProjectileAttackSkill.CanReturn;
        public bool IsTargetLocked => ProjectileAttackSkill.IsTargetLocked;


        Collider2D _collider;
        Rigidbody2D _rigidbody;
        SpriteRenderer _renderer;

        CancellationTokenSource _cts;

        readonly List<string> _damaged = new();
        int _penetrateLeft;
        int _chainLeft;
        float _durationLeft;

        bool _isTargetLocked;
        bool _isReturning;
        bool _isFreeze;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        public override void SetSkill(AttackSkill skill)
        {
            base.SetSkill(skill);
            if (skill is ProjectileAttackSkill projectileAttackSkill)
            {
                _penetrateLeft = (int)projectileAttackSkill.PenetrateCount.Value;
                _chainLeft = (int)projectileAttackSkill.ChainCount.Value;
                _isTargetLocked = projectileAttackSkill.IsTargetLocked;
            }
        }

        void ResetDuration()
        {
            _durationLeft = Duration.Value;
        }


        // TODO 逻辑优化
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out Damageable damageable) || !damageable.CompareTag(TargetTag))
            {
                return;
            }

            if (_isTargetLocked && Target != null && !Target.GetComponentInChildren<Damageable>().Equals(damageable))
            {
                // Debug.Log("not target");
                return;
            }

            if (!damageable.IsDamageable && Target != null && Target.GetComponentInChildren<Damageable>().Equals(damageable))
            {
                FindNewTarget();
                return;
            }

            if (_damaged.Contains(damageable.ID)) // 不能对同一个敌人造成多次伤害
            {
                // Debug.Log("already damaged");
                return;
            }

            ApplyDamage(damageable);
        }

        void ApplyDamage(IDamageable damageable)
        {
            _damaged.Add(damageable.ID);

            var damage = new AttackDamage(this, damageable, Keywords, DamageType.Simple, Damage.BaseValue, 1f, 1f);
            // Debug.Log($"Base Damage: {Damage.BaseValue}");
            damage.Apply();
            // Debug.Log($"Skill Damage: {Damage.GetValueByKeywords(Keywords)}");

            if (_isReturning)
            {
                return;
            }

            // 如果目标未锁定，且有穿透次数，则穿透
            if (!_isTargetLocked && _penetrateLeft > 0)
            {
                _penetrateLeft--;
                return;
            }
            else if (_chainLeft > 0)
            {
                _chainLeft--;
                if (Chain())
                {
                    return;
                }
            }
            else if (SplitCount.Value > 0)
            {
                Split();
                return;
            }
            else if (CanReturn)
            {
                Return();
                return;
            }

            Cancel();
        }

        void Split()
        {
            return;
        }

        bool Chain()
        {
            FindNewTarget();
            _isTargetLocked = true;

            Direction = (Target.position - transform.position).normalized;
            transform.right = Direction;
            return true;
        }

        void Return()
        {
            _isReturning = true;
            Target = AttackerController.CharacterController.CharacterModel.Transform;
            Direction = Target.position - transform.position;
            transform.right = Direction;
            _isTargetLocked = true;
        }

        void FindNewTarget()
        {
            Target = this.GetSystem<PositionQuerySystem>().QueryClosest(TargetTag, transform.position, _damaged);
            if (Target == null)
            {
                Cancel();
            }
        }

        void Move()
        {
            if (_isFreeze)
            {
                return;
            }

            if (_isTargetLocked)
            {
                if (Target == null)
                {
                    Target = this.GetSystem<PositionQuerySystem>().QueryClosest(TargetTag, transform.position, _damaged);
                    if (Target == null)
                    {
                        Cancel();
                        return;
                    }
                }

                Direction = ((Vector2)(Target.position - transform.position)).normalized;
                if ((transform.position - Target.position).sqrMagnitude > _collisionRadius * _collisionRadius)
                {
                    if (_collider.enabled)
                    {
                        _collider.enabled = false;
                    }
                }
                else
                {
                    if (!_collider.enabled)
                    {
                        _collider.enabled = true;
                    }
                }
            }

            _rigidbody.MovePosition(_rigidbody.position + ProjectileSpeed.Value * Time.fixedDeltaTime * Direction);
            _rigidbody.MoveRotation(_rigidbody.rotation + _rotateSpeed * 360 * Time.fixedDeltaTime);
        }

        protected override async UniTask Play(CancellationToken cancellationToken)
        {
            if (Target != null)
            {
                Direction = Target.position - transform.position;
            }
            else
            {
                Direction = transform.right;
            }

            // 方向产生一定随机性
            float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Direction = (Direction + _randomDirectionFactor * Direction.magnitude * randomDirection).normalized;
            transform.right = Direction;

            ResetDuration();

            while (!_isReturning && _durationLeft > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Move();
                _durationLeft -= Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate(cancellationToken);
            }
        }

        public override async UniTaskVoid Attack()
        {
            _cts = GlobalCancellation.GetCombinedTokenSource(this);

            // TODO 暂时性处理 WoodOnUse
            if (WoodOnUse.Value > 0)
            {
                if (AttackerController.CharacterController.CharacterModel is IHasResources resourceModel)
                {
                    this.GetSystem<ResourceSystem>().AcquireResource(ResourceType.Wood, (int)WoodOnUse.Value, resourceModel);
                }
            }

            try
            {
                await Play(_cts.Token);

                if (CanReturn && !_isReturning)
                {
                    Return();
                }

                if (_isReturning)
                {
                    while (Vector2.SqrMagnitude(Target.position - transform.position) > 0.1f)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        await UniTask.WaitForFixedUpdate(_cts.Token);
                    }
                }

                Cancel();
            }
            catch (OperationCanceledException)
            {

            }

        }

        public override void Cancel()
        {
            _cts.Cancel();
            AttackerController?.RemoveAttacker(this);
        }
    }
}
