using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
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

    public void AcquireSkill(ISkill skill)
    {
        this.SendCommand(new AcquireSkillCommand(skill));
    }

    public void ReleaseSkill(ISkill skill)
    {
        this.SendCommand(new ReleaseSkillCommand(skill));
    }

    public void RemoveSkill(string id)
    {
        this.SendCommand(new RemoveSkillCommand(id));
    }

    void OnSkillAcquired(SkillAcquiredEvent e)
    {
        if (e.Skill is not ActiveSkill activeSkill)
        {
            return;
        }

        _activeSkills.TryAdd(activeSkill.ID, activeSkill);
    }

    void OnSkillReleased(SkillReleasedEvent e)
    {
    }

    void OnSkillRemoved(SkillRemovedEvent e)
    {
        _activeSkills.Remove(e.SkillID);
    }

    void Awake()
    {
        _skillCreateSystem = this.GetSystem<SkillCreateSystem>();
        _attackerController = GetComponentInChildren<IAttackerController>();
    }

    void Start()
    {
        _model = this.GetModel<PlayersModel>().Current();
        this.SendCommand(new SetSkillSlotCountCommand(_skillSlotCount));
        this.RegisterEvent<SkillAcquiredEvent>(OnSkillAcquired);
        this.RegisterEvent<SkillReleasedEvent>(OnSkillReleased);
        this.RegisterEvent<SkillRemovedEvent>(OnSkillRemoved);

        CreateInitSkills().Forget();
    }

    async UniTaskVoid CreateInitSkills()
    {
        foreach (string skillID in _skillIDs)
        {
            SkillCreateEnv env = new(_attackerController, _model, this.GetSystem<ModifierSystem>());
            ISkill s = _skillCreateSystem.CreateSkill(skillID, env);
            AcquireSkill(s);

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
