using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Config;
using GamePlay.Character.Player;
using GamePlay.Skill;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillGachaUIController : MonoBehaviour, IController
{
    [SerializeField] AssetReferenceGameObject _skillGachaUIPrefab;

    SkillGachaSystem _skillGachaSystem;
    PlayerModel _model;


    List<SkillConfig> _gachaSkills = new();
    List<SkillGachaUI> _skillGachaUIs = new();

    [Button]
    public void GachaSkills()
    {
        _gachaSkills = _skillGachaSystem.GachaSkills(_model, 3);
    }

    public void SelectSkill(int index)
    {
        _skillGachaSystem.SelectSkill(_model, _gachaSkills, index);
    }

    public void CancelSelect()
    {
        _skillGachaSystem.CancelSelect(_model, _gachaSkills);
    }

    async UniTask CreateSkillGachaUI(int index, SkillConfig skill)
    {
        GameObject obj = await Addressables.InstantiateAsync(_skillGachaUIPrefab, transform);
        obj.transform.localPosition = Vector3.zero + new Vector3(index * 200, 0, 0);
        obj.transform.localScale = Vector3.one;

        SkillGachaUI skillGachaUI = obj.GetComponent<SkillGachaUI>();
        skillGachaUI.SetSkill(skill);
        skillGachaUI.Index = index;
        skillGachaUI.OnSelect.Register(SelectSkill).UnRegisterWhenDisabled(this);
        _skillGachaUIs.Add(skillGachaUI);
    }

    void SelectSkillGachaUI(int index)
    {
        for (int i = 0; i < _skillGachaUIs.Count; i++)
        {
            Addressables.ReleaseInstance(_skillGachaUIs[i].gameObject);
        }

        _skillGachaUIs.Clear();
    }

    void Awake()
    {
        _skillGachaSystem = this.GetSystem<SkillGachaSystem>();

    }

    void Start()
    {
        _model = this.GetModel<PlayersModel>().Current;

        this.RegisterEvent<GachaSkillsEvent>(e =>
        {
            if (e.Model != _model)
            {
                return;
            }

            for (int i = 0; i < e.Skills.Count; i++)
            {
                CreateSkillGachaUI(i, e.Skills[i]).Forget();
            }
        }).UnRegisterWhenDisabled(this);

        this.RegisterEvent<SelectSkillEvent>(e =>
        {
            if (e.Model != _model)
            {
                return;
            }

            SelectSkillGachaUI(e.Index);

        }).UnRegisterWhenDisabled(this);
    }

    public IArchitecture GetArchitecture()
    {
        return GameFrame.Interface;
    }
}
