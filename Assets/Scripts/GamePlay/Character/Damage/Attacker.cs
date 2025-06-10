using Character.Stat;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Character.Damage
{
    public interface IAttacker
    {
        IAttackerController AttackerController { get; set; }
        int BaseDamage { get; set; }
        int BaseCriticalChance { get; set; }
        int BaseCriticalMultiplier { get; set; }
        int BaseArea { get; set; }
        int BaseDuration { get; set; }

        IKeywordStat Damage { get; }
        IStat CriticalChance { get; }
        IStat CriticalMultiplier { get; }
        IStat AttackArea { get; }
        IStat Duration { get; }

        UniTaskVoid Attack();
        void Cancel();
    }



    public abstract class Attacker : MonoBehaviour, IAttacker, IController
    {
        public IAttackerController AttackerController { get; set; }

        public int BaseDamage { get; set; }
        public int BaseCriticalChance { get; set; }
        public int BaseCriticalMultiplier { get; set; }
        public int BaseArea { get; set; }
        public int BaseDuration { get; set; }

        public IKeywordStat Damage { get; protected set; }
        public IStat CriticalChance { get; protected set; }
        public IStat CriticalMultiplier { get; protected set; }
        public IStat AttackArea { get; protected set; }
        public IStat Duration { get; protected set; }

        public void SetStats(Stats stats)
        {
            Damage = stats.GetStat("Damage") as IKeywordStat;
            CriticalChance = stats.GetStat("CriticalChance");
            CriticalMultiplier = stats.GetStat("CriticalMultiplier");
            AttackArea = stats.GetStat("AttackArea");
            Duration = stats.GetStat("Duration");

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
