using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttackerController : ICharacterControlled
    {
        UniTask<IAttacker> CreateAttacker(string skillID, string attackerID);
        void RemoveAttacker(IAttacker attacker);
        void ClearAttacker();
        bool CanAttack { get; set; }
    }

    public abstract class AttackerController : CharacterControlled, IController, IAttackerController
    {
        protected ICharacterModel Model => CharacterController.CharacterModel;
        public bool CanAttack { get; set; } = true;
        protected List<IAttacker> Attackers = new();
        AttackerCreateSystem _attackerCreateSystem;

        protected override void OnInit()
        {
            _attackerCreateSystem = this.GetSystem<AttackerCreateSystem>();
        }

        protected override void OnDeinit()
        {
        }

        public async UniTask<IAttacker> CreateAttacker(string skillID, string attackerID)
        {
            IAttacker attacker = await CreateAttackerInternal(skillID, attackerID);
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

        protected async UniTask<IAttacker> GetOrCreateAttacker(string skillID, string attackerID)
        {
            if (!CharacterController.CharacterModel.TryGetSkill(skillID, out ISkill skill))
            {
                Debug.LogError($"未获得ID为{skillID}的技能");
                return null;
            }

            if (skill is not AttackSkill attackSkill)
            {
                Debug.LogError($"ID为{skillID}的技能不是攻击技能");
                return null;
            }

            IAttacker attacker = null;
            if (attackerID == "self")
            {
                attacker = GetComponentInChildren<IAttacker>();
                attacker?.SetSkill(attackSkill);
            }
            else
            {
                attacker = await _attackerCreateSystem.CreateAttacker(attackSkill, attackerID, transform);
            }

            return attacker;
        }

        protected abstract UniTask<IAttacker> CreateAttackerInternal(string skillID, string attackerID);

        public virtual void ClearAttacker()
        {
            Attackers.Clear();
            Attackers = new();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }

}
