using System.Text;
using Character.Player;
using Skill;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SkillUI : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _skillsDescription;

        StringBuilder _skillsDescriptionBuilder = new();

        void Awake()
        {
            this.RegisterEvent<SkillAcquiredEvent>(e =>
            {
                if (e.Model is PlayerModel playerModel)
                {
                    _skillsDescriptionBuilder.Clear();
                    _skillsDescriptionBuilder.AppendLine("已装备技能：");
                    foreach (ISkill skill in playerModel.SkillsInSlot.GetAllSkills())
                    {
                        _skillsDescriptionBuilder.AppendLine($"  {skill.Name}");
                    }

                    _skillsDescriptionBuilder.AppendLine("已吞噬技能：");
                    foreach (ISkill skill in playerModel.SkillsReleased.GetAllSkills())
                    {
                        _skillsDescriptionBuilder.AppendLine($"  {skill.Name}");
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