using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

public class EffectCreateSystem : AbstractSystem
{
    readonly Dictionary<string, SkillEffectInfo> _effectInfoCache = new();
    const string JsonPath = "Preset";
    const string JsonName = "Effects.json";

    public class EffectCreateEnv
    {
        public IAttackerController AttackerController;
        public CharacterModel Model;

        public EffectCreateEnv(IAttackerController attackerController, CharacterModel model)
        {
            AttackerController = attackerController;
            Model = model;
        }
    }

    void Load()
    {
        _effectInfoCache.Clear();
        List<SkillEffectInfo> effectInfoList = this.GetUtility<SaveLoadUtility>().Load<List<SkillEffectInfo>>(JsonName, JsonPath);
        foreach (SkillEffectInfo effectInfo in effectInfoList)
        {
            _effectInfoCache.Add(effectInfo.ID, effectInfo);
        }
    }

    public SkillEffectInfo GetEffectInfo(string id)
    {
        if (_effectInfoCache.TryGetValue(id, out SkillEffectInfo effectInfo))
        {
            return effectInfo;
        }

        Debug.LogError($"SkillEffectInfo not found: {id}");
        return null;
    }

    public T GetEffectInfo<T>(string id) where T : SkillEffectInfo
    {
        SkillEffectInfo effectInfo = GetEffectInfo(id);
        if (effectInfo == null)
        {
            return null;
        }

        if (effectInfo is not T t)
        {
            Debug.LogError($"EffectInfo type mismatch: {id}, {effectInfo.GetType()}, {typeof(T)}");
            return null;
        }

        return t;
    }

    // // 将字符串转为类型使用反射，更灵活
    // public ISkillEffect<SkillEffectInfo> CreateEffect(string id)
    // {
    //     SkillEffectInfo effectInfo = GetEffectInfo(id);
    //     if (effectInfo == null)
    //     {
    //         return null;
    //     }

    //     var effectType = Type.GetType(effectInfo.EffectType);

    //     return (ISkillEffect<SkillEffectInfo>)Activator.CreateInstance(effectType, effectInfo);
    // }

    // // 泛型控制反射，更安全
    // public TEffect CreateEffect<TInfo, TEffect>(string id) where TInfo : SkillEffectInfo where TEffect : ISkillEffect<TInfo>
    // {
    //     TInfo effectInfo = GetEffectInfo<TInfo>(id);
    //     if (effectInfo == null)
    //     {
    //         return default;
    //     }

    //     return (TEffect)Activator.CreateInstance(typeof(TEffect), effectInfo);
    // }

    public IEffect CreateEffect(string id, EffectCreateEnv env)
    {
        SkillEffectInfo skillInfo = GetEffectInfo(id);
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
        IStatModifier modifier = this.GetSystem<ModifierSystem>().CreateStatModifier(info.ModifierID, factory, info.Value);
        return new ModifierEffect(info, modifier);
    }

    protected override void OnInit()
    {
        Load();
    }
}
