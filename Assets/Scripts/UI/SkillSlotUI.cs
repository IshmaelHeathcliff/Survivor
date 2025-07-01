using System.Text;
using GamePlay.Character.Player;
using GamePlay.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image _icon;
        [SerializeField] TextMeshProUGUI _name;
        [SerializeField] Button _button;

        public EasyEvent<ISkill> OnSkillPointerEnter = new();
        public EasyEvent<ISkill> OnSkillPointerExit = new();
        public EasyEvent<ISkill> OnSkillReplace = new();

        public ISkill Skill { get; private set; }

        public bool IsRemovable
        {
            get => _button.interactable;
            set
            {
                if (Skill == null)
                {
                    value = false;
                }

                _button.interactable = value;
            }
        }

        void OnReplace()
        {
            if (Skill == null)
            {
                return;
            }

            IsRemovable = false;
            OnSkillReplace.Trigger(Skill);
        }


        public void SetSkill(ISkill skill = null)
        {
            if (skill == null)
            {
                Skill = null;
                _name.text = "";
                return;
            }

            Skill = skill;
            _name.text = skill.Name;
        }

        void OnValidate()
        {
            _name = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _icon = transform.Find("Icon").GetComponent<Image>();
            _button = GetComponent<Button>();
        }

        void Awake()
        {
            _button.onClick.AddListener(OnReplace);
            IsRemovable = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnSkillPointerEnter.Trigger(Skill);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnSkillPointerExit.Trigger(Skill);
        }
    }
}
