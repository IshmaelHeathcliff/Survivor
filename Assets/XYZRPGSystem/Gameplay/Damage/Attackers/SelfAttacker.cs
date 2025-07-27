using System.Collections.Generic;
using System.Threading;
using XYZRPGSystem.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XYZRPGSystem.Gameplay.Character;
using System;
using Gameplay.Character.Player;

namespace XYZRPGSystem.Gameplay.Damage.Attackers
{
    [RequireComponent(typeof(MoveController))]
    public class SelfAttacker : Attacker
    {
        Collider2D _collider;
        MoveController _moveController;

        CancellationTokenSource _cts;


        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _moveController = GetComponent<MoveController>();
        }


        public async UniTask AttackMove(CancellationToken ct)
        {
            float attackSpeed = AttackerController.CharacterController.CharacterStats.GetKeywordStat("AttackSpeed").Value;

            Vector3 initialPosition = _moveController.Position;
            Vector2 targetPosition = this.GetSystem<PositionQuerySystem>().QueryPosition(TargetTag);

            while (Vector2.Distance(targetPosition, _moveController.Position) > 0.1f)
            {
                ct.ThrowIfCancellationRequested();
                Vector2 velocity = (targetPosition - _moveController.Position).normalized * attackSpeed;
                _moveController.MoveTo(_moveController.Position + velocity * Time.fixedDeltaTime);
                await UniTask.WaitForFixedUpdate(ct);
            }


            while (Vector2.Distance(initialPosition, _moveController.Position) > 0.1f)
            {
                ct.ThrowIfCancellationRequested();
                _moveController.MoveTo(Vector2.Lerp(_moveController.Position, initialPosition, attackSpeed * Time.fixedDeltaTime));
                await UniTask.WaitForFixedUpdate(ct);
            }

            _moveController.MoveTo(initialPosition);
        }

        protected override async UniTask Play(CancellationToken ct)
        {
            try
            {
                _collider.enabled = true;
                enabled = true;
                await AttackMove(ct);
                enabled = false;
                _collider.enabled = false;
            }
            catch (OperationCanceledException)
            {
            }
        }

        public override async UniTaskVoid Attack()
        {
            _cts = GlobalCancellation.GetCombinedTokenSource(this);

            try
            {
                await Play(_cts.Token);
            }
            catch (OperationCanceledException)
            {
            }

        }

        public override void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (AttackerController == null || !AttackerController.CanAttack)
            {
                return;
            }

            if (!other.TryGetComponent(out Damageable damageable) || !damageable.IsDamageable)
            {
                // Debug.Log("not damageable");
                return;
            }

            if (!damageable.CompareTag(TargetTag))
            {
                // Debug.Log("not target tag");
                return;
            }


            var damage = new AttackDamage(this, damageable, Keywords, DamageType.Simple, Damage.BaseValue, 1, 1);
            damage.Apply();
        }

        void OnDestroy()
        {
            Cancel();
        }

    }
}
