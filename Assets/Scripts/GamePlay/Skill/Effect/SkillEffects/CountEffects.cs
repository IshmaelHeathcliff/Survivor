using Character.Modifier;
using System;
using System.Collections.Generic;

public class CountIncrementEffect : NestedSkillEffect<CountIncrementEffectConfig>
{
    CountSystem _countSystem;
    int _lastTriggerValue;
    CountIncrementEffectConfig _countConfig;

    public CountIncrementEffect(CountIncrementEffectConfig config, IEnumerable<IEffect> childEffects, CountSystem system) : base(config, childEffects)
    {
        _countSystem = system;
        _countConfig = config;
    }

    protected override void OnApply()
    {
        if (_countSystem == null)
        {
            return;
        }

        _countSystem.Register(_countConfig.CountValueID, OnCountValueChanged);
        _lastTriggerValue = _countSystem.GetCount(_countConfig.CountValueID); // 初始化上一次触发时的计数
    }

    protected override void OnCancel()
    {
        if (_countSystem != null)
        {
            _countSystem.Unregister(_countConfig.CountValueID, OnCountValueChanged);
        }

    }

    void OnCountValueChanged(int count)
    {
        // 检查计数是否达到或超过阈值
        if (count - _lastTriggerValue >= _countConfig.Increment)
        {
            foreach (IEffect childEffect in ChildEffects)
            {
                childEffect.Apply();
            }

            _lastTriggerValue = count; // 更新上一次触发时的计数
        }
    }
}
