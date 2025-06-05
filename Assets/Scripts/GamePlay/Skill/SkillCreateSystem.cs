using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

public class SkillCreateSystem : AbstractSystem
{
    readonly Dictionary<string, SkillInfo> _skillInfoCache = new();
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
        _skillInfoCache.Clear();
        List<SkillInfo> skillInfoList = this.GetUtility<SaveLoadUtility>().Load<List<SkillInfo>>(JsonName, JsonPath);
        foreach (SkillInfo skillInfo in skillInfoList)
        {
            _skillInfoCache.Add(skillInfo.ID, skillInfo);
        }
    }

    public SkillInfo GetSkillInfo(string id)
    {
        if (_skillInfoCache.TryGetValue(id, out SkillInfo skillInfo))
        {
            return skillInfo;
        }

        Debug.LogError($"SkillInfo not found: {id}");
        return null;
    }

    #region Effect
    public IEffect CreateEffect(SkillEffectInfo skillInfo, EffectCreateEnv env)
    {
        return skillInfo switch
        {
            AttackEffectInfo attackEffectInfo => CreateAttackEffect(attackEffectInfo, env.AttackerController),
            ModifierEffectInfo modifierEffectInfo => CreateModifierEffect(modifierEffectInfo, env.Model.Stats),
            _ => null,
        };
    }

    public AttackEffect CreateAttackEffect(AttackEffectInfo info, IAttackerController controller)
    {
        return new AttackEffect(info, controller);
    }

    public ModifierEffect CreateModifierEffect(ModifierEffectInfo info, IStatModifierFactory factory)
    {
        return new ModifierEffect(info, this.GetSystem<ModifierSystem>(), factory);
    }

    public SystemEffect CreateSystemEffect(SystemEffectInfo info, AbstractSystem system)
    {
        return new SystemEffect(info, system);
    }
    #endregion



    #region Skill
    public ISkill CreateSkill(string id, EffectCreateEnv env)
    {
        SkillInfo skillInfo = GetSkillInfo(id);
        if (skillInfo == null)
        {
            return default;
        }

        List<IEffect> skillEffectsOnEnable = new();

        foreach (SkillEffectInfo effectInfo in skillInfo.SkillEffectInfosOnEnable)
        {
            skillEffectsOnEnable.Add(CreateEffect(effectInfo, env));
        }

        switch (skillInfo)
        {
            case ActiveSkillInfo activeSkillInfo:
                List<IEffect> skillEffectsOnUpdate = new();
                foreach (SkillEffectInfo effectInfo in activeSkillInfo.SkillEffectInfosOnUpdate)
                {
                    skillEffectsOnUpdate.Add(CreateEffect(effectInfo, env));
                }
                return new ActiveSkill(activeSkillInfo, skillEffectsOnEnable, skillEffectsOnUpdate);
            case PassiveSkillInfo passiveSkillInfo:
                return new PassiveSkill(passiveSkillInfo, skillEffectsOnEnable);
            default:
                Debug.LogError($"未知的技能类型: {skillInfo.GetType()}");
                return default;
        }
    }
    #endregion

    protected override void OnInit()
    {
        Load();
    }
}
