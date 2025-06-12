using Character.Damage;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Character.Player
{
    public class PlayerAttackerController : AttackerController
    {
        [SerializeField] float _attackInterval;
        PlayerInput.PlayerActions _playerInput;
        AttackerCreateSystem _attackerCreateSystem;

        protected override async UniTask<IAttacker> CreateAttackerInternal(string skillID, string attackerID)
        {
            Vector2 playerPos = this.SendQuery(new PlayerPositionQuery());
            Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos).normalized;

            IAttacker attacker = await GetOrCreateAttacker(skillID, attackerID);

            float angle = Random.Range(0, 2 * Mathf.PI);
            var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            attacker.Direction = (direction + randomDirection * direction.magnitude / 2).normalized;
            transform.DetachChildren();

            return attacker;
        }

        void AttackAction(InputAction.CallbackContext context)
        {
            // Attack().Forget();
        }

        void RegisterActions()
        {
            _playerInput.Attack.performed += AttackAction;
        }

        void UnregisterActions()
        {
            _playerInput.Attack.performed -= AttackAction;
        }

        void OnEnable()
        {
            _playerInput = this.GetSystem<InputSystem>().PlayerActionMap;
            RegisterActions();
        }

        void OnDisable()
        {
            UnregisterActions();
        }
    }
}
