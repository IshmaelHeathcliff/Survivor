using System.Collections.Generic;
using UnityEngine;

public class RollDiceEffect : NestedSkillEffect<RollDiceEffectConfig>
{
    readonly CountSystem _countSystem;
    public RollDiceEffect(RollDiceEffectConfig skillEffectConfig, IEnumerable<IEffect> childEffects, CountSystem countSystem) : base(skillEffectConfig, childEffects)
    {
        _countSystem = countSystem;
    }

    protected override void OnApply()
    {
        int value = Random.Range(1, 6);
        _countSystem.IncrementCount("RollDice", value);

        foreach (IEffect childEffect in ChildEffects)
        {
            if (childEffect is IEffect<int> effect)
            {
                effect.Apply(value);
            }
            else
            {
                childEffect.Apply();
            }
        }
    }
}
