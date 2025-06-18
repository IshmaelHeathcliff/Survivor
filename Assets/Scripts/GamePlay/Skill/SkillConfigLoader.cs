using System.Collections.Generic;
using Data.Config;
using GamePlay.Skill.Effect;
using UnityEngine;


namespace GamePlay.Skill
{
    public class SkillCreationContext
    {
        public ISkill Skill { get; set; }
        public SkillCreateEnv Env { get; set; }
    }

    public class SkillConfigLoader
    {
        readonly SkillEffectConfigLoader _skillEffectConfigLoader = new();

        List<IEffect> CreateSkillEffects(IEnumerable<SkillEffectConfig> effectConfigs, SkillCreationContext context)
        {
            List<IEffect> skillEffects = new();

            if (effectConfigs == null)
            {
                return skillEffects;
            }

            foreach (SkillEffectConfig effectConfig in effectConfigs)
            {
                IEffect effect = _skillEffectConfigLoader.CreateEffect(effectConfig, context);
                if (effect != null)
                {
                    skillEffects.Add(effect);
                }
                else
                {
                    Debug.LogError($"创建技能效果失败: {effectConfig.GetType()}");
                }
            }
            return skillEffects;
        }

        public ISkill CreateSkill(SkillConfig skillConfig, SkillCreateEnv env)
        {
            var context = new SkillCreationContext { Env = env };

            List<IEffect> skillEffectsOnEnable = new();
            List<IEffect> skillEffectsOnUpdate = new();

            switch (skillConfig)
            {
                case AttackSkillConfig attackSkillConfig:
                    context.Skill = new AttackSkill(attackSkillConfig, env.Model.Stats);
                    skillEffectsOnUpdate.AddRange(CreateSkillEffects(attackSkillConfig.AttackEffectConfigs, context));
                    break;
                case RepetitiveSkillConfig repetitiveSkillConfig:
                    context.Skill = new RepetitiveSkill(repetitiveSkillConfig, env.Model.Stats);
                    break;
                case OneTimeSkillConfig oneTimeSkillConfig:
                    context.Skill = new OneTimeSkill(oneTimeSkillConfig, env.Model.Stats);
                    break;
                default:
                    Debug.LogError($"未知的技能类型: {skillConfig.GetType()}");
                    return null;
            }

            skillEffectsOnEnable.AddRange(CreateSkillEffects(skillConfig.SkillEffectConfigsOnEnable, context));
            skillEffectsOnUpdate.AddRange(CreateSkillEffects(skillConfig.SkillEffectConfigsOnUse, context));

            context.Skill.SetEffects(skillEffectsOnEnable, skillEffectsOnUpdate);

            return context.Skill;
        }
    }

    public class SkillEffectConfigLoader
    {
        public IEffect CreateEffect(SkillEffectConfig skillConfig, SkillCreationContext context)
        {
            IEffect effect = skillConfig switch
            {
                AttackEffectConfig attackEffectConfig => new AttackEffect(attackEffectConfig, context.Env.Model),
                ModifierEffectConfig modifierEffectConfig => new ModifierEffect(modifierEffectConfig, context.Env.Model, context.Env.ModifierSystem),
                AcquireResourceEffectConfig acquireResourceEffectConfig => new AcquireResourceEffect(acquireResourceEffectConfig, context.Env.Model, context.Env.ResourceSystem),
                NestedEffectConfig nestedEffectConfig => CreateNestedEffect(nestedEffectConfig, context),
                AcquireSkillEffectConfig acquireSkillEffectConfig => new AcquireSkillEffect(acquireSkillEffectConfig, context.Env.Model, context.Env.SkillSystem),
                _ => null,
            };

            if (effect != null)
            {
                effect.Owner = context.Skill;
            }

            return effect;
        }

        public IEffect CreateNestedEffect(NestedEffectConfig skillConfig, SkillCreationContext context)
        {
            if (skillConfig?.ChildEffects == null || context?.Env == null)
            {
                Debug.LogError("创建嵌套技能效果失败: skillConfig或context为null");
                return null;
            }

            List<IEffect> childEffects = new();
            foreach (SkillEffectConfig effectConfig in skillConfig.ChildEffects)
            {
                IEffect childEffect = CreateEffect(effectConfig, context);
                if (childEffect != null)
                {
                    childEffects.Add(childEffect);
                }
                else
                {
                    Debug.LogError($"创建子技能效果失败: {effectConfig.GetType()}, 描述: {effectConfig.Description ?? "无描述"}");
                }
            }

            IEffect effect = skillConfig switch
            {
                RollDiceEffectConfig rollDiceEffectConfig => new RollDiceEffect(rollDiceEffectConfig, context.Env.Model, childEffects, context.Env.CountSystem),
                OnRandomValueEffectConfig onRandomValueEffectConfig => new OnRandomValueEffect(onRandomValueEffectConfig, context.Env.Model, childEffects),
                CountIncrementEffectConfig countIncrementEffectConfig => new CountIncrementEffect(countIncrementEffectConfig, context.Env.Model, childEffects, context.Env.CountSystem),
                OnValueEffectConfig diceOnValueEffectConfig => new OnValueEffect(diceOnValueEffectConfig, context.Env.Model, childEffects),
                not null => new NestedSkillEffect<NestedEffectConfig>(skillConfig, context.Env.Model, childEffects),
            };

            effect.Owner = context.Skill;

            return effect;
        }
    }
}
