using System;
using System.Collections.Generic;
using SaveLoad;
using UnityEngine;

public class SkillCreateSystem : AbstractSystem
{
    readonly Dictionary<string, SkillInfo> _skillInfoCache = new();
    const string JsonPath = "Preset";
    const string JsonName = "Skills.json";

    EffectCreateSystem _effectCreateSystem;

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

    public ISkill CreateSkill(string id, EffectCreateSystem.EffectCreateEnv env)
    {
        SkillInfo skillInfo = GetSkillInfo(id);
        if (skillInfo == null)
        {
            return default;
        }

        List<IEffect> skillEffectsOnUpdate = new();
        List<IEffect> skillEffectsOnEnable = new();

        foreach (string effectID in skillInfo.SkillEffectIDsOnUpdate)
        {
            skillEffectsOnUpdate.Add(_effectCreateSystem.CreateEffect(effectID, env));
        }

        foreach (string effectID in skillInfo.SkillEffectIDsOnEnable)
        {
            skillEffectsOnEnable.Add(_effectCreateSystem.CreateEffect(effectID, env));
        }

        switch (skillInfo)
        {
            case ActiveSkillInfo activeSkillInfo:
                return new ActiveSkill(activeSkillInfo, skillEffectsOnUpdate, skillEffectsOnEnable);
            case PassiveSkillInfo passiveSkillInfo:
                return new PassiveSkill(passiveSkillInfo, skillEffectsOnUpdate, skillEffectsOnEnable);
            default:
                Debug.LogError($"未知的技能类型: {skillInfo.GetType()}");
                return default;
        }
    }

    // 使用反射创建技能
    // public ISkill CreateSkillReflection(string id)
    // {
    //     SkillInfo skillInfo = GetSkillInfo(id);
    //     if (skillInfo == null)
    //     {
    //         return default;
    //     }

    //     List<IEffect> skillEffectsOnUpdate = new();
    //     List<IEffect> skillEffectsOnEnable = new();

    //     foreach (string effectID in skillInfo.SkillEffectIDsOnUpdate)
    //     {
    //         skillEffectsOnUpdate.Add(_effectCreateSystem.CreateEffect(effectID));
    //     }

    //     foreach (string effectID in skillInfo.SkillEffectIDsOnEnable)
    //     {
    //         skillEffectsOnEnable.Add(_effectCreateSystem.CreateEffect(effectID));
    //     }

    //     var skillType = Type.GetType(skillInfo.SkillType);

    //     return (ISkill)Activator.CreateInstance(skillType, skillInfo, skillEffectsOnUpdate, skillEffectsOnEnable);
    // }

    protected override void OnInit()
    {
        Load();
        _effectCreateSystem = this.GetSystem<EffectCreateSystem>();
    }
}
