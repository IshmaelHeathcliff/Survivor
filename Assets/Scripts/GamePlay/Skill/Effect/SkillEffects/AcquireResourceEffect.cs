using Data.Config;

namespace GamePlay.Skill.Effect
{
    public class AcquireResourceEffect : SkillEffect<AcquireResourceEffectConfig>, IEffect<int>
    {
        readonly ResourceSystem _resourceSystem;
        int _acquireValue;

        public AcquireResourceEffect(AcquireResourceEffectConfig config, ResourceSystem resourceSystem) : base(config)
        {
            _resourceSystem = resourceSystem;
            Description = $"获取资源 {config.ResourceID} {config.Amount}";
        }

        protected override void OnApply()
        {
            _acquireValue += SkillEffectConfig.Amount;
            _resourceSystem.AcquireResource(SkillEffectConfig.ResourceID, SkillEffectConfig.Amount);
        }

        protected override void OnCancel()
        {
            _resourceSystem.AcquireResource(SkillEffectConfig.ResourceID, -_acquireValue);
        }

        public void Apply(int value)
        {
            _acquireValue += value;
            _resourceSystem.AcquireResource(SkillEffectConfig.ResourceID, value);
        }

        public void Cancel(int value)
        {
            _resourceSystem.AcquireResource(SkillEffectConfig.ResourceID, -_acquireValue);
        }
    }
}
