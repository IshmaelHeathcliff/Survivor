using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SkillController : MonoBehaviour, IController
{
    [SerializeField] string[] _skillIDs;
    [SerializeField] int _skillSlotCount;
    readonly Dictionary<string, ActiveSkill> _activeSkills = new();

    SkillCreateSystem _skillCreateSystem;
    IAttackerController _attackerController;
    ICharacterModel _model;

    bool AddSkillToSlot(ISkill skill)
    {
        ISkillContainer skillsInSlot = _model.SkillsInSlot;
        if (skillsInSlot.HasSkill(skill.ID))
        {
            Debug.Log($"技能 {skill.ID} 已经在槽位中");
            return false;
        }

        if (skillsInSlot.Count >= _skillSlotCount)
        {
            Debug.Log($"技能槽位已满，当前数量: {skillsInSlot.Count}，最大数量: {_skillSlotCount}");
            return false;
        }

        skillsInSlot.AddSkill(skill);
        return true;
    }

    bool RemoveSkillFromSlot(string id)
    {
        ISkillContainer skillsInSlot = _model.SkillsInSlot;
        if (skillsInSlot.HasSkill(id))
        {
            skillsInSlot.RemoveSkill(id);
            return true;
        }

        return false;
    }

    public void ReleaseSkill(string id)
    {
        RemoveSkillFromSlot(id);
    }


    public void AddSkill(ISkill skill)
    {
        if (!AddSkillToSlot(skill))
        {
            return;
        }

        if (!_model.Skills.AddSkill(skill))
        {
            RemoveSkillFromSlot(skill.ID);
            return;
        }

        if (skill is ActiveSkill activeSkill)
        {
            _activeSkills.TryAdd(activeSkill.ID, activeSkill);
        }
    }


    public void RemoveSkill(ISkill skill)
    {
        if (!_model.Skills.RemoveSkill(skill.ID))
        {
            return;
        }

        RemoveSkillFromSlot(skill.ID);

        if (skill is ActiveSkill activeSkill)
        {
            _activeSkills.Remove(activeSkill.ID);
        }

    }


    void Awake()
    {
        _skillCreateSystem = this.GetSystem<SkillCreateSystem>();
        _attackerController = GetComponentInChildren<IAttackerController>();
    }

    async void Start()
    {
        _model = this.GetModel<PlayersModel>().Current();
        foreach (string skill in _skillIDs)
        {
            SkillCreateSystem.EffectCreateEnv env = new(_attackerController, _model);
            ISkill s = _skillCreateSystem.CreateSkill(skill, env);
            AddSkill(s);

            await UniTask.Delay(TimeSpan.FromSeconds(0.1));
        }
    }

    void Update()
    {
        foreach (ActiveSkill skill in _activeSkills.Values)
        {
            skill.Update(Time.deltaTime);

            // 自动释放
            if (skill.IsReady)
            {
                skill.Use();
            }
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameFrame.Interface;
    }
}
