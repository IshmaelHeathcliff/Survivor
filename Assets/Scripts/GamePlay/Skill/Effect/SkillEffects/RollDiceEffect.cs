using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Skill.Effect
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

    public class DiceOnValueEffect : NestedSkillEffect<DiceOnValueEffectConfig>, IEffect<int>
    {
        public int Roll { get; set; }

        public DiceOnValueEffect(DiceOnValueEffectConfig skillEffectConfig, IEnumerable<IEffect> childEffects) : base(skillEffectConfig, childEffects)
        {
            Roll = skillEffectConfig.Value;
        }

        protected override void OnApply()
        {
            foreach (IEffect childEffect in ChildEffects)
            {
                childEffect.Apply();
            }
        }

        protected override void OnCancel()
        {
            foreach (IEffect childEffect in ChildEffects)
            {
                childEffect.Cancel();
            }
        }

        public void Apply(int value)
        {
            if (value == Roll)
            {
                foreach (IEffect childEffect in ChildEffects)
                {
                    childEffect.Apply();
                }
            }
        }

        public void Cancel(int value)
        {
            if (value == Roll)
            {
                foreach (IEffect childEffect in ChildEffects)
                {
                    childEffect.Cancel();
                }
            }
        }
    }
}
