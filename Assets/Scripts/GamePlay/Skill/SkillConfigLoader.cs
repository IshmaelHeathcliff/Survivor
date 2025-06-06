using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using UnityEngine;

public class SkillCreateEnv
{
    public IAttackerController AttackerController;
    public ICharacterModel Model;
    public ModifierSystem ModifierSystem;

    public SkillCreateEnv(IAttackerController attackerController, ICharacterModel model, ModifierSystem modifierSystem)
    {
        AttackerController = attackerController;
        Model = model;
        ModifierSystem = modifierSystem;
    }
}


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
            AttackEffectConfig attackEffectConfig => CreateAttackEffect(attackEffectConfig, env.AttackerController),
            ModifierEffectConfig modifierEffectConfig => CreateModifierEffect(modifierEffectConfig, env.ModifierSystem, env.Model.Stats),
            _ => null,
        };
    }

    public static AttackEffect CreateAttackEffect(AttackEffectConfig config, IAttackerController controller)
    {
        if (controller == null)
        {
            Debug.LogError($"Create AttackEffect failed, AttackerController is null");
            return null;
        }

        return new AttackEffect(config, controller);
    }

    public static ModifierEffect CreateModifierEffect(ModifierEffectConfig config, ModifierSystem modifierSystem, IStatModifierFactory factory)
    {
        if (factory == null)
        {
            Debug.LogError($"Create ModifierEffect failed, IStatModifierFactory is null");
            return null;
        }

        return new ModifierEffect(config, modifierSystem, factory);
    }

    public static SystemEffect CreateSystemEffect(SystemEffectConfig config, AbstractSystem system)
    {
        return new SystemEffect(config, system);
    }
}