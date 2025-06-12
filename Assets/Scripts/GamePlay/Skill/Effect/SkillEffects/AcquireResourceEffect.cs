namespace Skill.Effect
{
    public class AcquireResourceEffect : SkillEffect<AcquireResourceConfig>, IEffect<int>
    {
        ResourceSystem _resourceSystem;
        AcquireResourceConfig _config;

        public AcquireResourceEffect(AcquireResourceConfig skillEffectConfig, ResourceSystem resourceSystem) : base(skillEffectConfig)
        {
            _resourceSystem = resourceSystem;
            _config = skillEffectConfig;
        }

        public void Apply(int value)
        {
            _resourceSystem.AcquireResource(_config.ResourceID, value);
        }

        public void Cancel(int value)
        {
        }

        protected override void OnApply()
        {
            _resourceSystem.AcquireResource(_config.ResourceID, _config.Amount);
        }

        protected override void OnCancel()
        {
        }

    }
}
