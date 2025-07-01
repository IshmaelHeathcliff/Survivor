using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using GamePlay.Character.Player;
using GamePlay.Character.Stat;
using GamePlay.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public class SkillReleasedUIController : MonoBehaviour, IController
    {
        [SerializeField] AssetReferenceGameObject _skillReleasedUIReference;
        [SerializeField] GameObject _skillInfo;
        TextMeshProUGUI _skillInfoText;
        PlayerModel _playerModel;

        List<SkillReleasedUI> _skillReleasedUIs = new();

        void Init()
        {
            foreach (ISkill skill in _playerModel.SkillsReleased.GetAllSkills())
            {
                CreateSkillReleasedUI(skill).Forget();
            }
        }


        async UniTaskVoid CreateSkillReleasedUI(ISkill skill)
        {
            GameObject obj = await _skillReleasedUIReference.InstantiateAsync(transform);
            SkillReleasedUI skillReleasedUI = obj.GetComponent<SkillReleasedUI>();
            skillReleasedUI.SetSkill(skill);
            _skillReleasedUIs.Add(skillReleasedUI);

            skillReleasedUI.OnSkillPointerEnter.Register(OnPointerEnter).UnRegisterWhenDisabled(skillReleasedUI);
            skillReleasedUI.OnSkillPointerExit.Register(OnPointerExit).UnRegisterWhenDisabled(skillReleasedUI);
        }

        void OnSkillReleased(SkillReleasedEvent e)
        {
            if (e.Model is PlayerModel playerModel && playerModel.SkillsReleased.HasSkill(e.Skill.ID))
            {
                CreateSkillReleasedUI(e.Skill).Forget();
            }
        }

        void OnPointerEnter(ISkill skill)
        {
            // _skillInfo.SetActive(true);

            if (skill == null)
            {
                _skillInfoText.text = "";
                return;
            }
            else
            {
                var info = new StringBuilder();

                if (skill is AttackSkill attackSkill)
                {
                    info.Append(SkillStats.GenerateSkillStatInfo(attackSkill));
                }
                else
                {
                    info.Append(skill.Name);
                    info.AppendLine(": \n\n");
                    info.Append(skill.Description);
                }

                _skillInfoText.text = info.ToString();
            }
        }

        void OnPointerExit(ISkill skill)
        {
            // _skillInfo.SetActive(false);
            // _skillInfoText.text = "";
        }

        void Awake()
        {
            _skillInfoText = _skillInfo.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            _playerModel = this.GetModel<PlayersModel>().Current;

            this.RegisterEvent<SkillReleasedEvent>(OnSkillReleased).UnRegisterWhenGameObjectDestroyed(gameObject);

            Init();

        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
