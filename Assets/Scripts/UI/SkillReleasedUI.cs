using System.Text;
using GamePlay.Character.Player;
using GamePlay.Skill;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillReleasedUI : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _skillsDescription;

        StringBuilder _skillsDescriptionBuilder = new();

        void Awake()
        {
            this.RegisterEvent<SkillReleasedEvent>(e =>
            {
                if (e.Model is PlayerModel playerModel)
                {
                    _skillsDescriptionBuilder.Clear();
                    foreach (ISkill skill in playerModel.SkillsReleased.GetAllSkills())
                    {
                        _skillsDescriptionBuilder.Append($"{skill.Name}, ");
                    }

                    _skillsDescription.text = _skillsDescriptionBuilder.ToString();

                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
