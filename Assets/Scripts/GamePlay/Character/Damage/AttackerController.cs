using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamePlay.Skill;
using UnityEngine;

namespace GamePlay.Character.Damage
{
    public interface IAttackerController : ICharacterControlled
    {
        UniTask<IEnumerable<IAttacker>> CreateAttackers(string skillID, string attackerID);
        void RemoveAttacker(IAttacker attacker);
        void ClearAttacker();
        bool CanAttack { get; set; }
    }

    public abstract class AttackerController : CharacterControlled, IController, IAttackerController
    {
        [SerializeField] protected string TargetTag;
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
            ClearAttacker();
        }

        public async UniTask<IEnumerable<IAttacker>> CreateAttackers(string skillID, string attackerID)
        {
            var attackers = (await CreateAttackerInternal(skillID, attackerID)).ToList();
            foreach (IAttacker attacker in attackers)
            {
                attacker.AttackerController = this;
                attacker.TargetTag = TargetTag;

                if (!Attackers.Contains(attacker))
                {
                    Attackers.Add(attacker);
                }
            }

            return attackers;
        }

        public void RemoveAttacker(IAttacker attacker)
        {
            Attackers.Remove(attacker);
        }

        protected async UniTask<IEnumerable<IAttacker>> GetOrCreateAttacker(string skillID, string attackerID)
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

            IEnumerable<IAttacker> attackers;
            if (attackerID == "self")
            {
                attackers = GetComponentsInChildren<IAttacker>();
            }
            else
            {
                attackers = await _attackerCreateSystem.CreateAttacker(attackSkill, attackerID, transform);
            }

            return attackers;
        }

        protected abstract UniTask<IEnumerable<IAttacker>> CreateAttackerInternal(string skillID, string attackerID);

        public virtual void ClearAttacker()
        {
            var attackers = Attackers.ToList();
            foreach (IAttacker attacker in attackers)
            {
                attacker.Cancel().Forget();
            }
            Attackers.Clear();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }

}
