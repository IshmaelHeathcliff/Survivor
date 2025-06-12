using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using UnityEngine;

public class SkillCreationContext
{
    public ISkill Skill { get; set; }
    public SkillCreateEnv Env { get; set; }
}

public static class SkillConfigLoader
{
    static List<IEffect> CreateSkillEffects(IEnumerable<SkillEffectConfig> effectConfigs, SkillCreationContext context)
    {
        List<IEffect> skillEffects = new();

        if (effectConfigs == null)
        {
            return skillEffects;
        }

        foreach (SkillEffectConfig effectConfig in effectConfigs)
        {
            IEffect effect = SkillEffectConfigLoader.CreateEffect(effectConfig, context);
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

    public static ISkill CreateSkill(SkillConfig skillConfig, SkillCreateEnv env)
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
                skillEffectsOnUpdate.AddRange(CreateSkillEffects(repetitiveSkillConfig.SkillEffectConfigsOnUpdate, context));
                break;
            case OneTimeSkillConfig oneTimeSkillConfig:
                context.Skill = new OneTimeSkill(oneTimeSkillConfig, env.Model.Stats);
                break;
            default:
                Debug.LogError($"未知的技能类型: {skillConfig.GetType()}");
                return default;
        }

        skillEffectsOnEnable.AddRange(CreateSkillEffects(skillConfig.SkillEffectConfigsOnEnable, context));

        context.Skill.SetEffects(skillEffectsOnEnable, skillEffectsOnUpdate);

        return context.Skill;
    }
}

public static class SkillEffectConfigLoader
{
    public static IEffect CreateEffect(SkillEffectConfig skillConfig, SkillCreationContext context)
    {
        IEffect effect = skillConfig switch
        {
            AttackEffectConfig attackEffectConfig => new AttackEffect(attackEffectConfig, context.Env.Model.Controller.AttackerController),
            LocalModifierEffectConfig localModifierEffectConfig => new ModifierEffect(localModifierEffectConfig, context.Env.ModifierSystem, context.Env.Model.GetSkill(localModifierEffectConfig.AttackSkillID).SkillStats),
            ModifierEffectConfig modifierEffectConfig => new ModifierEffect(modifierEffectConfig, context.Env.ModifierSystem, context.Env.Model.Stats),
            AcquireResourceConfig acquireResourceEffectConfig => new AcquireResourceEffect(acquireResourceEffectConfig, context.Env.ResourceSystem),
            NestedEffectConfig nestedEffectConfig => CreateNestedEffect(nestedEffectConfig, context),
            _ => null,
        };

        if (effect != null)
        {
            effect.Owner = context.Skill;
        }

        return effect;
    }

    public static IEffect CreateNestedEffect(NestedEffectConfig skillConfig, SkillCreationContext context)
    {
        if (skillConfig?.ChildEffects == null || context?.Env == null)
        {
            Debug.LogError($"创建嵌套技能效果失败: skillConfig或context为null");
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
            RollDiceEffectConfig rollDiceEffectConfig => new RollDiceEffect(rollDiceEffectConfig, childEffects, context.Env.CountSystem, context.Env.Model),
            CountIncrementEffectConfig countIncrementEffectConfig => new CountIncrementEffect(countIncrementEffectConfig, childEffects, context.Env.CountSystem, context.Env.Model),
            DiceOnValueEffectConfig diceOnValueEffectConfig => new DiceOnValueEffect(diceOnValueEffectConfig, childEffects),
            NestedEffectConfig nestedEffectConfig => new NestedSkillEffect<NestedEffectConfig>(nestedEffectConfig, childEffects),
            _ => null,
        };

        if (effect == null)
        {
            Debug.LogError($"未知的嵌套技能效果类型: {skillConfig.GetType()}, 描述: {skillConfig.Description ?? "无描述"}");
        }
        else
        {
            effect.Owner = context.Skill;
        }

        return effect;
    }
}
