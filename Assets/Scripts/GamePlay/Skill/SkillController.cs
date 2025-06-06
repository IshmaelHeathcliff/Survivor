using System;
using System.Collections.Generic;
using Character;
using Character.Damage;
using Character.Modifier;
using Character.Player;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillController : MonoBehaviour, IController
{
    [SerializeField] string[] _skillIDs;
    [SerializeField] int _skillSlotCount;
    readonly Dictionary<string, ActiveSkill> _activeSkills = new();

    IAttackerController _attackerController;
    ICharacterModel _model;

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

    [Button]
    void CreateSkill(string skillID)
    {
        this.SendCommand(new AcquireSkillCommand(skillID));
    }


    async UniTaskVoid CreateInitSkills()
    {
        foreach (string skillID in _skillIDs)
        {
            this.SendCommand(new AcquireSkillCommand(skillID));
            await UniTask.Delay(TimeSpan.FromSeconds(0.1));
        }
    }

    void Awake()
    {
        _attackerController = GetComponentInChildren<IAttackerController>();
    }

    void Start()
    {
        _model = this.GetModel<PlayersModel>().Current;

        // ! 初始化技能系统
        this.SendCommand(new SetSkillEnvCommand(_attackerController, _model));
        this.SendCommand(new SetSkillSlotCountCommand(_skillSlotCount));

        this.RegisterEvent<SkillAcquiredEvent>(OnSkillAcquired);
        this.RegisterEvent<SkillReleasedEvent>(OnSkillReleased);
        this.RegisterEvent<SkillRemovedEvent>(OnSkillRemoved);

        CreateInitSkills().Forget();
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
