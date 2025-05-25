using Character.Damage;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Player
{
    public class PlayerAttackerController : AttackerController
    {
        [SerializeField] float _attackInterval;
        [SerializeField] GameObject _playerAttacker;
        PlayerInput.PlayerActions _playerInput;

        protected override IAttacker GetOrCreateAttackerInternal()
        {
            Vector3 playerPos = this.SendQuery(new PlayerPositionQuery());
            PlayerAttacker attacker = Instantiate(_playerAttacker, transform).GetComponent<PlayerAttacker>();
            attacker.Direction = ((Vector2)(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos)).normalized;
            attacker.SetStats(CharacterController.Stats);
            transform.DetachChildren();
            return attacker;
        }

        async UniTaskVoid Attack()
        {
            if (!CanAttack)
            {
                return;
            }

            CanAttack = false;
            GetOrCreateAttacker();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackInterval));
            CanAttack = true;
        }

        void Update()
        {
            Attack().Forget();
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
