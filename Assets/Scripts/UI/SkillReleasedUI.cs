using Gameplay.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SkillReleasedUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image _icon;

        public EasyEvent<ISkill> OnSkillPointerEnter = new();
        public EasyEvent<ISkill> OnSkillPointerExit = new();

        public ISkill Skill { get; private set; }

        public void SetSkill(ISkill skill = null)
        {
            if (skill == null)
            {
                Skill = null;
                _icon.gameObject.SetActive(false);
                return;
            }

            _icon.gameObject.SetActive(true);
            if (_icon.sprite != null)
            {
                Addressables.Release(_icon.sprite);
            }

            Skill = skill;
            Addressables.LoadAssetAsync<Sprite>(skill.IconAddress).Completed += (handle) =>
            {
                _icon.sprite = handle.Result;
            };

        }

        void OnValidate()
        {
            _icon = transform.Find("Icon").GetComponent<Image>();
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
