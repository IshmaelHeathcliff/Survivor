using Character;

namespace Skill.Effect
{
    public class AcquireSkillEffect : SkillEffect<AcquireSkillEffectConfig>
    {
        readonly SkillSystem _skillSystem;
        readonly ICharacterModel _model;

        public AcquireSkillEffect(AcquireSkillEffectConfig config, SkillSystem skillSystem, ICharacterModel model) : base(config)
        {
            _skillSystem = skillSystem;
            _model = model;
            Description = $"获取技能 {config.SkillID}";
        }

        protected override void OnApply()
        {
            _skillSystem.AcquireSkill(SkillEffectConfig.SkillID, _model);
        }

        protected override void OnCancel()
        {
            _skillSystem.RemoveSkill(SkillEffectConfig.SkillID, _model);
        }
    }
}
