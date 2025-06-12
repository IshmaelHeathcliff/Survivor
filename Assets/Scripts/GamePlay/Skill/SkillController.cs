using System;
using System.Collections.Generic;
using Character;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Skill
{
    [RequireComponent(typeof(ICharacterController))]
    public class SkillController : MonoBehaviour, IController
    {
        [SerializeField] string[] _skillIDs;
        [SerializeField] int _skillSlotCount;
        readonly Dictionary<string, RepetitiveSkill> _repetitiveSkills = new();

        ICharacterController _characterController;
        ICharacterModel _model;

        void OnSkillAcquired(SkillAcquiredEvent e)
        {
            if (e.Model != _model)
            {
                return;
            }

            if (e.Skill is not RepetitiveSkill repetitiveSkill)
            {
                return;
            }

            _repetitiveSkills.TryAdd(repetitiveSkill.ID, repetitiveSkill);
        }

        void OnSkillReleased(SkillReleasedEvent e)
        {
            if (e.Model != _model)
            {
                return;
            }
        }

        void OnSkillRemoved(SkillRemovedEvent e)
        {
            if (e.Model != _model)
            {
                return;
            }

            _repetitiveSkills.Remove(e.SkillID);
        }

        [Button]
        void CreateSkill(string skillID)
        {
            this.SendCommand(new AcquireSkillCommand(skillID, _model));
        }


        async UniTaskVoid CreateInitSkills()
        {
            foreach (string skillID in _skillIDs)
            {
                this.SendCommand(new AcquireSkillCommand(skillID, _model));
                await UniTask.Delay(TimeSpan.FromSeconds(0.1));
            }
        }

        void Awake()
        {
            _characterController = GetComponent<ICharacterController>();
        }

        void Start()
        {
            _model = _characterController.CharacterModel;

            // ! 初始化技能系统
            this.SendCommand(new SetSkillSlotCountCommand(_skillSlotCount, _model));

            this.RegisterEvent<SkillAcquiredEvent>(OnSkillAcquired).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<SkillReleasedEvent>(OnSkillReleased).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<SkillRemovedEvent>(OnSkillRemoved).UnRegisterWhenGameObjectDestroyed(gameObject);

            CreateInitSkills().Forget();
        }


        void Update()
        {
            foreach (RepetitiveSkill skill in _repetitiveSkills.Values)
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
}
