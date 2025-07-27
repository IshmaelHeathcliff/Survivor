using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using XYZRPGSystem.Gameplay.Character;
using Gameplay.Character.Player;

namespace Gameplay.Character.Enemy
{
    public class EnemyMoveController : MoveController
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Chase = Animator.StringToHash("Chase");
        public static readonly int Patrol = Animator.StringToHash("Patrol");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int Dead = Animator.StringToHash("Dead");

        public void Move()
        {
            Rigidbody.MovePosition(Rigidbody.position + Speed * Time.fixedDeltaTime * Direction);
        }

        public Vector2 DirectionToPlayer()
        {
            Vector2 playerPos = this.SendQuery(new PlayerPositionQuery());
            Vector2 direction = playerPos - Rigidbody.position;
            return direction;
        }

        public float SqrDistanceToPlayer()
        {
            return DirectionToPlayer().sqrMagnitude;
        }

        public bool FindPlayer()
        {
            Vector2 direction = DirectionToPlayer();
            Face(direction);
            return true;
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
    }
}
