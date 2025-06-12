using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Damage;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = Core.InputSystem;
using Random = UnityEngine.Random;

namespace GamePlay.Character.Player
{
    public class PlayerAttackerController : AttackerController
    {
        [SerializeField] float _attackInterval;
        PlayerInput.PlayerActions _playerInput;
        AttackerCreateSystem _attackerCreateSystem;

        protected override async UniTask<IEnumerable<IAttacker>> CreateAttackerInternal(string skillID, string attackerID)
        {
            Vector2 playerPos = this.SendQuery(new PlayerPositionQuery());
            Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos).normalized;

            var attackers = (List<IAttacker>)await GetOrCreateAttacker(skillID, attackerID);

            foreach (IAttacker attacker in attackers)
            {
                float angle = Random.Range(0, 2 * Mathf.PI);
                var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                attacker.Direction = (direction + randomDirection * direction.magnitude / 2).normalized;
            }
            transform.DetachChildren();

            return attackers;
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
