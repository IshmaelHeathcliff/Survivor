using Character.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerDamageable : Damageable
    {
        protected override void OnInit()
        {
            base.OnInit();
            OnHurt = new EasyEvent();
            OnDeath = new EasyEvent();
        }


        void Start()
        {
            SetStats(CharacterController.CharaterStats);

            OnHurt.Register(() => { }).UnRegisterWhenDisabled(this);
            OnDeath.Register(() => Dead().Forget()).UnRegisterWhenDisabled(this);
        }

        async UniTaskVoid Dead()
        {
            IsDamageable = false;
            await UniTask.Delay((int)(1000 * 0.5f));
            (CharacterController as PlayerController).Respawn();
            await UniTask.Delay((int)(1000 * 0.5f));
            IsDamageable = true;
        }
    }
}
