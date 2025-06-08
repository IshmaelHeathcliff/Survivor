using System.Text;
using TMPro;
using UnityEngine;

public class SkillUI : MonoBehaviour, IController
{
    [SerializeField] TextMeshProUGUI _skillsDescription;

    StringBuilder _skillsDescriptionBuilder = new();

    void Awake()
    {
        this.RegisterEvent<SkillAcquiredEvent>(e =>
        {
            _skillsDescriptionBuilder.AppendLine(e.Skill.ID);
            _skillsDescription.text = _skillsDescriptionBuilder.ToString();
        });
    }

    public IArchitecture GetArchitecture()
    {
        return GameFrame.Interface;
    }
}
