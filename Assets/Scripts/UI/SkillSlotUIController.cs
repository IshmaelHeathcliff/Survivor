using System.Collections.Generic;
using System.Linq;
using GamePlay.Character.Player;
using GamePlay.Skill;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillSlotUIController : MonoBehaviour, IController
    {
        [SerializeField] List<SkillSlotUI> _skillSlotUIs;
        [SerializeField] GameObject _skillInfo;
        [SerializeField] TextMeshProUGUI _skillInfoText;

        PlayerModel _playerModel;

        void OnSkillAcquired(SkillAcquiredEvent e)
        {
            if (e.Model != _playerModel)
            {
                return;
            }

            if (!_playerModel.SkillsInSlot.HasSkill(e.Skill.ID))
            {
                return;
            }

            foreach (SkillSlotUI slot in _skillSlotUIs)
            {
                if (slot.Skill == null)
                {
                    slot.SetSkill(e.Skill);
                    break;
                }
            }
        }

        void OnSkillReleased(SkillReleasedEvent e)
        {
            if (e.Model != _playerModel)
            {
                return;
            }

            foreach (SkillSlotUI slot in _skillSlotUIs)
            {
                if (slot.Skill == e.Skill)
                {
                    slot.SetSkill();
                    break;
                }
            }

        }

        void OnPointerEnter(ISkill skill)
        {
            _skillInfo.gameObject.SetActive(true);
            _skillInfoText.text = skill.Description;
        }

        void OnPointerExit(ISkill skill)
        {
            _skillInfo.gameObject.SetActive(false);
            _skillInfoText.text = "";
        }

        void OnValidate()
        {
            _skillSlotUIs = GetComponentsInChildren<SkillSlotUI>().ToList();
            _skillInfo = transform.Find("SkillInfo").gameObject;
            _skillInfoText = _skillInfo.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            _playerModel = this.GetModel<PlayersModel>().Current;

            this.RegisterEvent<SkillAcquiredEvent>(OnSkillAcquired).UnRegisterWhenDisabled(this);
            this.RegisterEvent<SkillReleasedEvent>(OnSkillReleased).UnRegisterWhenDisabled(this);

            foreach (SkillSlotUI slot in _skillSlotUIs)
            {
                slot.OnSkillPointerEnter.Register(OnPointerEnter).UnRegisterWhenDisabled(this);
                slot.OnSkillPointerExit.Register(OnPointerExit).UnRegisterWhenDisabled(this);
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
