using UnityEngine;
using Data.Config;
using UnityEngine.UI;
using TMPro;
using System;
using GamePlay.Skill;
using GamePlay.Character.Player;

public class SkillGachaUI : MonoBehaviour, IController
{
    [SerializeField] TextMeshProUGUI _skillName;
    [SerializeField] Button _selectButton;

    public EasyEvent<int> OnSelect = new();

    public int Index { get; set; }

    SkillConfig _skill;

    public void SetSkill(SkillConfig skill)
    {
        _skill = skill;
        _skillName.text = skill.Name;
    }

    void OnValidate()
    {
        _selectButton = GetComponent<Button>();
        _skillName = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Awake()
    {
    }


    void Start()
    {
        _selectButton.onClick.AddListener(() => OnSelect.Trigger(Index));
    }

    public IArchitecture GetArchitecture()
    {
        return GameFrame.Interface;
    }
}
