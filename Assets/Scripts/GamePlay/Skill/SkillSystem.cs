using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using Character.Player;
using SaveLoad;
using UnityEngine;

public class SkillCreateEnv
{
    public IAttackerController AttackerController;
    public ICharacterModel Model;
    public ModifierSystem ModifierSystem;
    public ResourceSystem ResourceSystem;
    public CountSystem CountSystem;
    public SkillSystem SkillSystem;
}

public class SkillSystem : AbstractSystem
{
    readonly Dictionary<string, SkillConfig> _skillConfigCache = new();
    const string JsonPath = "Preset";
    const string JsonName = "Skills.json";

    public SkillCreateEnv SkillCreateEnv { get; set; }

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

    public void SetEnv(IAttackerController attackerController, ICharacterModel model)
    {
        SkillCreateEnv.AttackerController = attackerController;
        SkillCreateEnv.Model = model;
    }

    public ISkill CreateSkill(string id)
    {
        if (!CheckEnv())
        {
            return null;
        }

        return SkillConfigLoader.CreateSkill(GetSkillConfig(id), SkillCreateEnv);
    }

    public void AcquireSkill(string id)
    {
        if (!CheckEnv())
        {
            return;
        }

        ISkill skill = CreateSkill(id);

        ISkillContainer SkillsInSlot = SkillCreateEnv.Model.SkillsInSlot;
        ISkillContainer SkillsReleased = SkillCreateEnv.Model.SkillsReleased;

        if (SkillsInSlot.Count >= SkillsInSlot.MaxCount)
        {
            Debug.Log($"技能槽位已满，最大数量: {SkillsInSlot.MaxCount}");
            return;
        }

        if (!SkillsInSlot.AddSkill(skill))
        {
            return;
        }

        this.SendEvent(new SkillAcquiredEvent(skill, SkillsInSlot, SkillsReleased));
    }

    public void ReleaseSkill(string id)
    {
        if (!CheckEnv())
        {
            return;
        }

        ISkill skill = CreateSkill(id);

        if (!SkillCreateEnv.Model.SkillsInSlot.RemoveSkill(skill.ID))
        {
            return;
        }

        if (!SkillCreateEnv.Model.SkillsReleased.AddSkill(skill))
        {
            return;
        }

        this.SendEvent(new SkillReleasedEvent(skill, SkillCreateEnv.Model.SkillsInSlot, SkillCreateEnv.Model.SkillsReleased));
    }

    public void RemoveSkill(string id)
    {
        if (!CheckEnv())
        {
            return;
        }

        if (SkillCreateEnv.Model.SkillsInSlot.RemoveSkill(id) || SkillCreateEnv.Model.SkillsReleased.RemoveSkill(id))
        {
            this.SendEvent(new SkillRemovedEvent(id, SkillCreateEnv.Model.SkillsInSlot, SkillCreateEnv.Model.SkillsReleased));
        }
    }

    public void SetSkillSlotCount(int count)
    {
        if (!CheckEnv())
        {
            return;
        }

        SkillCreateEnv.Model.SkillSlotCount = count;
        this.SendEvent(new SkillSlotCountChangedEvent(count, SkillCreateEnv.Model.SkillsInSlot, SkillCreateEnv.Model.SkillsReleased));
    }

    public bool CheckEnv()
    {
        if (SkillCreateEnv.AttackerController == null)
        {
            Debug.LogError("AttackerController is null");
            return false;
        }

        if (SkillCreateEnv.Model == null)
        {
            Debug.LogError("Model is null");
            return false;
        }

        return true;
    }

    protected override void OnInit()
    {
        Load();
        SkillCreateEnv = new()
        {
            ModifierSystem = this.GetSystem<ModifierSystem>(),
            ResourceSystem = this.GetSystem<ResourceSystem>(),
            CountSystem = this.GetSystem<CountSystem>(),
            SkillSystem = this
        };
    }
}
