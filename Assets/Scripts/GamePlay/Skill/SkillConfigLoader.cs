using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using UnityEngine;



public static class SkillConfigLoader
{
    public static ISkill CreateSkill(SkillConfig skillConfig, SkillCreateEnv env)
    {

        List<IEffect> skillEffectsOnEnable = new();

        foreach (SkillEffectConfig effectConfig in skillConfig.SkillEffectConfigsOnEnable)
        {
            skillEffectsOnEnable.Add(SkillEffectConfigLoader.CreateEffect(effectConfig, env));
        }

        switch (skillConfig)
        {
            case ActiveSkillConfig activeSkillConfig:
                List<IEffect> skillEffectsOnUpdate = new();
                foreach (SkillEffectConfig effectConfig in activeSkillConfig.SkillEffectConfigsOnUpdate)
                {
                    skillEffectsOnUpdate.Add(SkillEffectConfigLoader.CreateEffect(effectConfig, env));
                }
                return new ActiveSkill(activeSkillConfig, skillEffectsOnEnable, skillEffectsOnUpdate);
            case PassiveSkillConfig passiveSkillConfig:
                return new PassiveSkill(passiveSkillConfig, skillEffectsOnEnable);
            default:
                Debug.LogError($"未知的技能类型: {skillConfig.GetType()}");
                return default;
        }
    }
}

public static class SkillEffectConfigLoader
{
    public static IEffect CreateEffect(SkillEffectConfig skillConfig, SkillCreateEnv env)
    {
        return skillConfig switch
        {
            AttackEffectConfig attackEffectConfig => new AttackEffect(attackEffectConfig, env.AttackerController),
            ModifierEffectConfig modifierEffectConfig => new ModifierEffect(modifierEffectConfig, env.ModifierSystem, env.Model.Stats),
            AcquireResourceConfig acquireResourceEffectConfig => new AcquireResourceEffect(acquireResourceEffectConfig, env.ResourceSystem),
            NestedEffectConfig nestedEffectConfig => CreateNestedEffect(nestedEffectConfig, env),
            _ => null,
        };
    }

    public static IEffect CreateNestedEffect(NestedEffectConfig skillConfig, SkillCreateEnv env)
    {
        List<IEffect> childEffects = new();
        foreach (SkillEffectConfig effectConfig in skillConfig.ChildEffects)
        {
            childEffects.Add(CreateEffect(effectConfig, env));
        }

        return skillConfig switch
        {
            RollDiceEffectConfig rollDiceEffectConfig => new RollDiceEffect(rollDiceEffectConfig, childEffects, env.CountSystem),
            CountIncrementEffectConfig countIncrementEffectConfig => new CountIncrementEffect(countIncrementEffectConfig, childEffects, env.CountSystem),
            _ => null,
        };
    }
}
