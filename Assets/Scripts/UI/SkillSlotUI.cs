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

        public EasyEvent<ISkill> OnSkillPointerEnter = new();
        public EasyEvent<ISkill> OnSkillPointerExit = new();

        public ISkill Skill { get; private set; }


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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Skill == null)
            {
                return;
            }

            OnSkillPointerEnter.Trigger(Skill);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Skill == null)
            {
                return;
            }

            OnSkillPointerExit.Trigger(Skill);
        }
    }
}
