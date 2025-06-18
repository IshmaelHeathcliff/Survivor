using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Damage;
using GamePlay.Character.Enemy;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = Core.InputSystem;

namespace GamePlay.Character.Player
{
    public class PlayerAttackerController : AttackerController
    {
        PlayerInput.PlayerActions _playerInput;

        protected override async UniTask<IEnumerable<IAttacker>> CreateAttackerInternal(string skillID, string attackerID)
        {
            // //向鼠标位置发射
            // Vector2 playerPos = this.SendQuery(new PlayerPositionQuery());
            // Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - playerPos).normalized;

            // var attackers = (List<IAttacker>)await GetOrCreateAttacker(skillID, attackerID);

            // foreach (IAttacker attacker in attackers)
            // {
            //     float angle = Random.Range(0, 2 * Mathf.PI);
            //     var randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            //     attacker.Direction = (direction + randomDirection * direction.magnitude / 2).normalized;
            // }
            // transform.DetachChildren();

            // 向最近敌人位置发射
            var attackers = (List<IAttacker>)await GetOrCreateAttacker(skillID, attackerID);
            List<string> selected = new();
            foreach (IAttacker attacker in attackers)
            {
                attacker.Target = this.GetSystem<PositionQuerySystem>().QueryClosest(TargetTag, transform.position, selected);
                if (attacker.Target != null)
                {
                    selected.Add(attacker.Target.GetComponentInChildren<Damageable>().ID);
                }
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
