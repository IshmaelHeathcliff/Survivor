using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Gameplay.Stat;

namespace XYZRPGSystem.Gameplay.Skill
{
    // 可以创建Attacker的Skill

    public abstract class AttackSkill : RepetitiveSkill
    {
        public IKeywordStat Damage => SkillStats.GetKeywordStat("Damage");
        public IKeywordStat CriticalChance => SkillStats.GetKeywordStat("CriticalChance");
        public IKeywordStat CriticalMultiplier => SkillStats.GetKeywordStat("CriticalMultiplier");
        public IKeywordStat AttackArea => SkillStats.GetKeywordStat("AttackArea");
        public IKeywordStat AttackRange => SkillStats.GetKeywordStat("AttackRange");
        public IKeywordStat Duration => SkillStats.GetKeywordStat("Duration");

        public IStat WoodOnUse => SkillStats.GetStat("WoodOnUse");


        public AttackSkill(AttackSkillConfig skillConfig, CharacterStats characterStats)
            : base(skillConfig, characterStats)
        {
            Damage.BaseValue = skillConfig.Damage;
            CriticalChance.BaseValue = skillConfig.CriticalChance;
            CriticalMultiplier.BaseValue = skillConfig.CriticalMultiplier;
            AttackArea.BaseValue = skillConfig.AttackArea;
            AttackRange.BaseValue = skillConfig.AttackRange;
            Duration.BaseValue = skillConfig.Duration;

            WoodOnUse.BaseValue = skillConfig.WoodOnUse;
        }
    }

    public class ProjectileAttackSkill : AttackSkill
    {
        public IKeywordStat ProjectileCount => SkillStats.GetKeywordStat("ProjectileCount");
        public IKeywordStat ProjectileSpeed => SkillStats.GetKeywordStat("ProjectileSpeed");
        public IKeywordStat ChainCount => SkillStats.GetKeywordStat("ChainCount");
        public IKeywordStat PenetrateCount => SkillStats.GetKeywordStat("PenetrateCount");
        public IKeywordStat SplitCount => SkillStats.GetKeywordStat("SplitCount");
        public bool IsTargetLocked { get; set; }
        public bool CanReturn { get; set; }
        public ProjectileAttackSkill(ProjectileAttackSkillConfig skillConfig, CharacterStats characterStats)
            : base(skillConfig, characterStats)
        {
            ProjectileCount.BaseValue = skillConfig.ProjectileCount;
            ProjectileSpeed.BaseValue = skillConfig.ProjectileSpeed;
            ChainCount.BaseValue = skillConfig.ChainCount;
            PenetrateCount.BaseValue = skillConfig.PenetrateCount;
            SplitCount.BaseValue = skillConfig.SplitCount;
            IsTargetLocked = skillConfig.IsTargetLocked;
            CanReturn = skillConfig.CanReturn;
        }
    }

    public class SelfAttackSkill : AttackSkill
    {
        public SelfAttackSkill(SelfAttackSkillConfig skillConfig, CharacterStats characterStats)
            : base(skillConfig, characterStats)
        {
        }
    }
}
