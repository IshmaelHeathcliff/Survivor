using Data.Config;
using GamePlay.Character.Stat;

namespace GamePlay.Skill
{
    // 可以创建Attacker的Skill

    public class AttackSkill : RepetitiveSkill
    {
        public IKeywordStat Damage => SkillStats.GetKeywordStat("Damage");
        public IKeywordStat CriticalChance => SkillStats.GetKeywordStat("CriticalChance");
        public IKeywordStat CriticalMultiplier => SkillStats.GetKeywordStat("CriticalMultiplier");
        public IKeywordStat AttackArea => SkillStats.GetKeywordStat("AttackArea");
        public IKeywordStat Duration => SkillStats.GetKeywordStat("Duration");
        public IKeywordStat ProjectileCount => SkillStats.GetKeywordStat("ProjectileCount");
        public IKeywordStat ProjectileSpeed => SkillStats.GetKeywordStat("ProjectileSpeed");

        public bool ReleaseOnAcquire { get; set; }

        public AttackSkill(AttackSkillConfig skillConfig, CharacterStats characterStats)
            : base(skillConfig, characterStats)
        {
            Damage.BaseValue = skillConfig.Damage;
            CriticalChance.BaseValue = skillConfig.CriticalChance;
            CriticalMultiplier.BaseValue = skillConfig.CriticalMultiplier;
            AttackArea.BaseValue = skillConfig.AttackArea;
            Duration.BaseValue = skillConfig.Duration;
            ProjectileCount.BaseValue = skillConfig.ProjectileCount;
            ProjectileSpeed.BaseValue = skillConfig.ProjectileSpeed;
            ReleaseOnAcquire = skillConfig.ReleaseOnAcquire;
        }
    }
}
