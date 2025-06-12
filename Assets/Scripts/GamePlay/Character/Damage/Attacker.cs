using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Stat;
using GamePlay.Skill;
using UnityEngine;

namespace GamePlay.Character.Damage
{
    public interface IAttacker
    {
        string ID { get; }
        IEnumerable<string> Keywords { get; }
        IAttackerController AttackerController { get; set; }
        Vector2 Direction { get; set; }

        IStat Damage { get; }
        IStat CriticalChance { get; }
        IStat CriticalMultiplier { get; }
        IStat AttackArea { get; }
        IStat Duration { get; }

        void SetSkill(AttackSkill skill);
        UniTaskVoid Attack();
        void Cancel();
    }



    public abstract class Attacker : MonoBehaviour, IAttacker, IController
    {
        public IAttackerController AttackerController { get; set; }
        AttackSkill _attackSkill;

        public string ID => _attackSkill.ID;
        public IEnumerable<string> Keywords => _attackSkill.Keywords;
        public Vector2 Direction { get; set; }


        public IStat Damage => _attackSkill.Damage;
        public IStat CriticalChance => _attackSkill.CriticalChance;
        public IStat CriticalMultiplier => _attackSkill.CriticalMultiplier;
        public IStat AttackArea => _attackSkill.AttackArea;
        public IStat Duration => _attackSkill.Duration;

        public void SetSkill(AttackSkill skill)
        {
            _attackSkill = skill;
        }

        protected abstract UniTask Play();
        public abstract UniTaskVoid Attack();
        public abstract void Cancel();

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
