using Character.Stat;

namespace Skill
{
    // 可以创建Attacker的Skill

    public class AttackSkill : RepetitiveSkill
    {
        public IStat Damage => SkillStats.GetStat("Damage");
        public IStat CriticalChance => SkillStats.GetStat("CriticalChance");
        public IStat CriticalMultiplier => SkillStats.GetStat("CriticalMultiplier");
        public IStat AttackArea => SkillStats.GetStat("AttackArea");
        public IStat Duration => SkillStats.GetStat("Duration");

        public AttackSkill(AttackSkillConfig skillConfig, CharacterStats characterStats)
            : base(skillConfig, characterStats)
        {
            Damage.BaseValue = skillConfig.Damage;
            CriticalChance.BaseValue = skillConfig.CriticalChance;
            CriticalMultiplier.BaseValue = skillConfig.CriticalMultiplier;
            AttackArea.BaseValue = skillConfig.AttackArea;
            Duration.BaseValue = skillConfig.Duration;
        }
    }
}
