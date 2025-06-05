using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

public class SkillCreateSystem : AbstractSystem
{
    readonly Dictionary<string, SkillConfig> _skillConfigCache = new();
    const string JsonPath = "Preset";
    const string JsonName = "Skills.json";


    public class EffectCreateEnv
    {
        public IAttackerController AttackerController;
        public ICharacterModel Model;

        public EffectCreateEnv(IAttackerController attackerController, ICharacterModel model)
        {
            AttackerController = attackerController;
            Model = model;
        }
    }

    void Load()
    {
        _skillConfigCache.Clear();
        List<SkillConfig> skillConfigList = this.GetUtility<SaveLoadUtility>().Load<List<SkillConfig>>(JsonName, JsonPath);
        foreach (SkillConfig skillConfig in skillConfigList)
        {
            _skillConfigCache.Add(skillConfig.ID, skillConfig);
        }
    }

    public SkillConfig GetSkillConfig(string id)
    {
        if (_skillConfigCache.TryGetValue(id, out SkillConfig skillConfig))
        {
            return skillConfig;
        }

        Debug.LogError($"SkillConfig not found: {id}");
        return null;
    }

    #region Effect
    public IEffect CreateEffect(SkillEffectConfig skillConfig, EffectCreateEnv env)
    {
        return skillConfig switch
        {
            AttackEffectConfig attackEffectConfig => CreateAttackEffect(attackEffectConfig, env.AttackerController),
            ModifierEffectConfig modifierEffectConfig => CreateModifierEffect(modifierEffectConfig, env.Model.Stats),
            _ => null,
        };
    }

    public AttackEffect CreateAttackEffect(AttackEffectConfig config, IAttackerController controller)
    {
        return new AttackEffect(config, controller);
    }

    public ModifierEffect CreateModifierEffect(ModifierEffectConfig config, IStatModifierFactory factory)
    {
        return new ModifierEffect(config, this.GetSystem<ModifierSystem>(), factory);
    }

    public SystemEffect CreateSystemEffect(SystemEffectConfig config, AbstractSystem system)
    {
        return new SystemEffect(config, system);
    }
    #endregion



    #region Skill
    public ISkill CreateSkill(string id, EffectCreateEnv env)
    {
        SkillConfig skillConfig = GetSkillConfig(id);
        if (skillConfig == null)
        {
            return default;
        }

        List<IEffect> skillEffectsOnEnable = new();

        foreach (SkillEffectConfig effectConfig in skillConfig.SkillEffectConfigsOnEnable)
        {
            skillEffectsOnEnable.Add(CreateEffect(effectConfig, env));
        }

        switch (skillConfig)
        {
            case ActiveSkillConfig activeSkillConfig:
                List<IEffect> skillEffectsOnUpdate = new();
                foreach (SkillEffectConfig effectConfig in activeSkillConfig.SkillEffectConfigsOnUpdate)
                {
                    skillEffectsOnUpdate.Add(CreateEffect(effectConfig, env));
                }
                return new ActiveSkill(activeSkillConfig, skillEffectsOnEnable, skillEffectsOnUpdate);
            case PassiveSkillConfig passiveSkillConfig:
                return new PassiveSkill(passiveSkillConfig, skillEffectsOnEnable);
            default:
                Debug.LogError($"未知的技能类型: {skillConfig.GetType()}");
                return default;
        }
    }
    #endregion

    protected override void OnInit()
    {
        Load();
    }
}
