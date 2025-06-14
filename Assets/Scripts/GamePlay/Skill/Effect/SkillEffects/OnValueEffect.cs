using System.Collections.Generic;
using GamePlay.Character;
using Data.Config;

namespace GamePlay.Skill.Effect
{

    public class OnValueEffect : NestedSkillEffect<OnValueEffectConfig>, IEffect<int>
    {
        public int Value { get; set; }

        public OnValueEffect(OnValueEffectConfig skillEffectConfig, ICharacterModel model, IEnumerable<IEffect> childEffects) : base(skillEffectConfig, model, childEffects)
        {
            Value = skillEffectConfig.Value;
            Description = $"当数值为 {Value} 时触发";
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
            if (value == Value)
            {
                foreach (IEffect childEffect in ChildEffects)
                {
                    childEffect.Apply();
                }
            }
        }

        public void Cancel(int value)
        {
            if (value == Value)
            {
                foreach (IEffect childEffect in ChildEffects)
                {
                    childEffect.Cancel();
                }
            }
        }
    }
}
