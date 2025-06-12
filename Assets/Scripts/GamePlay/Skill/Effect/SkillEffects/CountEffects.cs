using Character;
using System.Collections.Generic;

namespace Skill.Effect
{
    public class CountIncrementEffect : NestedSkillEffect<CountIncrementEffectConfig>
    {
        CountSystem _countSystem;
        int _lastTriggerValue;
        CountIncrementEffectConfig _countConfig;
        ICharacterModel _model;
        public CountIncrementEffect(CountIncrementEffectConfig config, IEnumerable<IEffect> childEffects, CountSystem system, ICharacterModel model) : base(config, childEffects)
        {
            _countSystem = system;
            _countConfig = config;
            _model = model;
        }

        protected override void OnApply()
        {
            if (_countSystem == null)
            {
                return;
            }

            _countSystem.Register(_countConfig.CountValueID, _model, OnCountValueChanged);
            _lastTriggerValue = _countSystem.GetCount(_countConfig.CountValueID, _model); // 初始化上一次触发时的计数
        }

        protected override void OnCancel()
        {
            if (_countSystem != null)
            {
                _countSystem.Unregister(_countConfig.CountValueID, _model, OnCountValueChanged);
            }

        }

        void OnCountValueChanged(CountChangedEvent e)
        {
            // 检查计数是否达到或超过阈值
            if (e.Value - _lastTriggerValue >= _countConfig.Increment)
            {
                foreach (IEffect childEffect in ChildEffects)
                {
                    childEffect.Apply();
                }

                _lastTriggerValue = e.Value; // 更新上一次触发时的计数
            }
        }
    }
}
