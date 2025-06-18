using System.Collections.Generic;
using Data.Config;
using GamePlay.Character;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public class OnRandomValueEffect : NestedSkillEffect<OnRandomValueEffectConfig>
    {
        public OnRandomValueEffect(OnRandomValueEffectConfig skillEffectConfig, ICharacterModel model, IEnumerable<IEffect> childEffects) : base(skillEffectConfig, model, childEffects)
        {
        }

        protected override void OnApply()
        {
            int value = Random.Range(SkillEffectConfig.Min, SkillEffectConfig.Max + 1);

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

    public class RollDiceEffect : OnRandomValueEffect
    {
        readonly CountSystem _countSystem;
        public RollDiceEffect(RollDiceEffectConfig skillEffectConfig, ICharacterModel model, IEnumerable<IEffect> childEffects, CountSystem countSystem) : base(skillEffectConfig, model, childEffects)
        {
            _countSystem = countSystem;
        }

        protected override void OnApply()
        {
            base.OnApply();
            _countSystem.IncrementCount("RollDiceTimes", Model, 1);
        }
    }
}
