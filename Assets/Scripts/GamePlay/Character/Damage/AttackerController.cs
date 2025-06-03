using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttackerController : ICharacterControlled
    {
        UniTask<IAttacker> CreateAttacker(string address);
        void RemoveAttacker(IAttacker attacker);
        void ClearAttacker();
        bool CanAttack { get; set; }
    }

    public abstract class AttackerController : CharacterControlled, IController, IAttackerController
    {
        protected ICharacterModel Model => CharacterController.Model;
        public bool CanAttack { get; set; } = true;
        protected List<IAttacker> Attackers = new();

        protected override void OnInit()
        {
        }

        protected override void OnDeinit()
        {
        }

        public async UniTask<IAttacker> CreateAttacker(string address = null)
        {
            IAttacker attacker = await GetOrCreateAttackerAsyncInternal(address);
            attacker.AttackerController = this;
            if (!Attackers.Contains(attacker))
            {
                Attackers.Add(attacker);
            }

            return attacker;
        }

        public void RemoveAttacker(IAttacker attacker)
        {
            Attackers.Remove(attacker);
        }

        protected abstract UniTask<IAttacker> GetOrCreateAttackerAsyncInternal(string address);

        public virtual void ClearAttacker()
        {
            Attackers.Clear();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }

}
