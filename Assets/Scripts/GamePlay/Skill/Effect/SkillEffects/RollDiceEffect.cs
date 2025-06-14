using System.Collections.Generic;
using Data.Config;
using GamePlay.Character;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public class RollDiceEffect : NestedSkillEffect<RollDiceEffectConfig>
    {
        readonly CountSystem _countSystem;
        readonly ICharacterModel _model;
        public RollDiceEffect(RollDiceEffectConfig skillEffectConfig, IEnumerable<IEffect> childEffects, CountSystem countSystem, ICharacterModel model) : base(skillEffectConfig, childEffects)
        {
            _countSystem = countSystem;
            _model = model;
        }

        protected override void OnApply()
        {
            int value = Random.Range(1, 6);
            _countSystem.IncrementCount("RollDice", _model, value);

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
