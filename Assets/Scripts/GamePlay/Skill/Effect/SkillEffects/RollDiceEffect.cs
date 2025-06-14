using System.Collections.Generic;
using Data.Config;
using GamePlay.Character;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public class RollDiceEffect : NestedSkillEffect<RollDiceEffectConfig>
    {
        readonly CountSystem _countSystem;
        public RollDiceEffect(RollDiceEffectConfig skillEffectConfig, ICharacterModel model, IEnumerable<IEffect> childEffects, CountSystem countSystem) : base(skillEffectConfig, model, childEffects)
        {
            _countSystem = countSystem;
        }

        protected override void OnApply()
        {
            int value = Random.Range(1, 6);
            _countSystem.IncrementCount("RollDice", Model, value);

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
}
