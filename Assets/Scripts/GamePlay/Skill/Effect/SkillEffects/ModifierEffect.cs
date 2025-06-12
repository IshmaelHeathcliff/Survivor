using System.Collections.Generic;
using Character.Modifier;

namespace Skill.Effect
{
    public class ModifierEffect : SkillEffect<ModifierEffectConfig>
    {
        readonly List<IStatModifier> _modifiers;
        readonly ModifierSystem _modifierSystem;
        readonly IStatModifierFactory _factory;

        public ModifierEffect(ModifierEffectConfig config, ModifierSystem modifierSystem, IStatModifierFactory factory) : base(config)
        {
            _modifiers = new List<IStatModifier>();
            _modifierSystem = modifierSystem;
            _factory = factory;
        }

        protected override void OnCancel()
        {
            foreach (var modifier in _modifiers)
            {
                modifier.Unregister();
            }
        }

        protected override void OnApply()
        {
            IStatModifier modifier = _modifierSystem.CreateStatModifier(SkillEffectConfig.ModifierID, _factory, SkillEffectConfig.Value);
            if (modifier != null)
            {
                _modifiers.Add(modifier);
                modifier.Register();
            }
        }
    }
}
