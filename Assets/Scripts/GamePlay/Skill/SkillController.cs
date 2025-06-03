using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Player;
using UnityEngine;

public class SkillController : MonoBehaviour, IController
{
    [SerializeField] string[] _skills;
    readonly Dictionary<string, ActiveSkill> _activeSkills = new();
    readonly Dictionary<string, PassiveSkill> _passiveSkills = new();

    SkillCreateSystem _skillCreateSystem;
    IAttackerController _attackerController;
    CharacterModel _model;

    public void AddActiveSkill(ActiveSkill skill)
    {
        if (_activeSkills.TryAdd(skill.SkillInfo.ID, skill))
        {
            skill.OnEnable();
        }
    }

    public void AddPassiveSkill(PassiveSkill skill)
    {
        if (_passiveSkills.TryAdd(skill.SkillInfo.ID, skill))
        {
            skill.OnEnable();
        }
    }

    public void RemoveActiveSkill(ActiveSkill skill)
    {
        if (_activeSkills.Remove(skill.SkillInfo.ID))
        {
            skill.OnDisable();
        }
    }

    public void RemovePassiveSkill(PassiveSkill skill)
    {
        if (_passiveSkills.Remove(skill.SkillInfo.ID))
        {
            skill.OnDisable();
        }
    }

    void Awake()
    {
        _skillCreateSystem = this.GetSystem<SkillCreateSystem>();
        _attackerController = GetComponentInChildren<IAttackerController>();
    }

    void Start()
    {
        _model = this.GetModel<PlayersModel>().Current();
        foreach (string skill in _skills)
        {
            EffectCreateSystem.EffectCreateEnv env = new(_attackerController, _model);
            ISkill s = _skillCreateSystem.CreateSkill(skill, env);
            if (s is ActiveSkill activeSkill)
            {
                AddActiveSkill(activeSkill);
            }
            else if (s is PassiveSkill passiveSkill)
            {
                AddPassiveSkill(passiveSkill);
            }
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
