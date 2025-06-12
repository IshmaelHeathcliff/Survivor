using System.Collections.Generic;
using Character;
using Character.Modifier;

namespace Skill.Effect
{
    public class ModifierEffect : SkillEffect<ModifierEffectConfig>
    {
        readonly List<IStatModifier> _modifiers = new();
        readonly ModifierSystem _modifierSystem;
        readonly ICharacterModel _model;

        public ModifierEffect(ModifierEffectConfig config, ModifierSystem modifierSystem, ICharacterModel model) : base(config)
        {
            _modifierSystem = modifierSystem;
            _model = model;
        }


        protected override void OnApply()
        {
            IStatModifierFactory factory;
            if (SkillEffectConfig is LocalModifierEffectConfig localConfig)
            {
                factory = _model.GetSkill(localConfig.AttackSkillID)?.SkillStats;
            }
            else
            {
                factory = _model.Stats;
            }

            IStatModifier modifier = _modifierSystem.CreateStatModifier(SkillEffectConfig.ModifierID, factory, SkillEffectConfig.Value);
            if (modifier != null)
            {
                _modifiers.Add(modifier);
                modifier.Register();
            }
        }


        protected override void OnCancel()
        {
            foreach (IStatModifier modifier in _modifiers)
            {
                modifier.Unregister();
            }

            _modifiers.Clear();
        }
    }
}
