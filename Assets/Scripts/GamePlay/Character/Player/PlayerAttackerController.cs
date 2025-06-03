using Character.Damage;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace Character.Player
{
    public class PlayerAttackerController : AttackerController
    {
        [SerializeField] float _attackInterval;
        PlayerInput.PlayerActions _playerInput;

        protected override async UniTask<IAttacker> GetOrCreateAttackerAsyncInternal(string address = null)
        {
            Vector2 playerPos = this.SendQuery(new PlayerPositionQuery());
            Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos).normalized;

            var obj = await Addressables.InstantiateAsync(address, transform)
                                        .ToUniTask(cancellationToken: GlobalCancellation.GetCombinedToken(this));
            PlayerAttacker attacker = obj.GetComponent<PlayerAttacker>();

            attacker.Direction = direction;
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
            await CreateAttacker("dice");
            await UniTask.Delay(TimeSpan.FromSeconds(_attackInterval));
            CanAttack = true;
        }

        void Update()
        {
            // Attack().Forget();
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
