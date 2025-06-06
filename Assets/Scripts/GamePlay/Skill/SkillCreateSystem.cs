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

    public ISkill CreateSkill(string id, SkillCreateEnv env)
    {
        return SkillConfigLoader.CreateSkill(GetSkillConfig(id), env);
    }

    protected override void OnInit()
    {
        Load();
    }
}
