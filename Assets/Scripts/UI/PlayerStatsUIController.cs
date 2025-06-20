﻿using System.Text;
using GamePlay.Character.Stat;
using GamePlay.Character.Player;
using Sirenix.OdinInspector;
using GamePlay.Skill;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerStatsUIController : MonoBehaviour, IController
    {
        [SerializeField] TextMeshProUGUI _text;

        PlayerModel _playerModel;

        [Button]
        // TODO: 优化属性更新方式，不再一次性更新所有
        void UpdateStatsInfo()
        {
            var info = new StringBuilder();
            info.Append(GenerateStatInfo(_playerModel.Stats.GetStat("Health")));
            info.Append(GenerateStatInfo(_playerModel.Stats.GetStat("HealthRegen")));
            info.Append(GenerateStatInfo(_playerModel.Stats.GetStat("MoveSpeed")));
            info.Append(GenerateStatInfo(_playerModel.Stats.GetStat("CoinGain")));
            info.Append(GenerateStatInfo(_playerModel.Stats.GetStat("WoodGain")));
            foreach (ISkill skill in _playerModel.GetAllSkills())
            {
                if (skill is AttackSkill attackSkill)
                {
                    info.Append(GenerateSkillStatInfo(attackSkill));
                }
            }
            _text.text = info.ToString();
        }

        StringBuilder GenerateSkillStatInfo(AttackSkill skill)
        {
            var info = new StringBuilder();
            info.Append($"{skill.Name}:\n");
            info.Append($"  Cooldown: {FormatStatValue(skill.Cooldown)}\n");
            foreach (IStat stat in skill.SkillStats.GetAllStats())
            {
                info.Append(GenerateStatInfo(stat, 1));
            }
            return info;
        }

        StringBuilder GenerateStatInfo(IStat stat, int indent = 0)
        {
            var info = new StringBuilder();
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"{stat.Name}: {FormatStatValue(stat.Value)}\n");
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"  {stat.Name}基础值: {FormatStatValue(stat.BaseValue)}\n");
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"  {stat.Name}附加值: {FormatStatValue(stat.AddedValue)}\n");
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"  {stat.Name}固定值: {FormatStatValue(stat.FixedValue)}\n");
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"  {stat.Name}提高: {(int)stat.Increase}%\n");
            info.Append($"{new string(' ', indent * 2)}");
            info.Append($"  {stat.Name}总增: {(int)((stat.More - 1) * 100)}%\n");
            return info;
        }

        string FormatStatValue(float value)
        {
            // 如果是整数或者很接近整数，显示为整数
            if (Mathf.Abs(value - Mathf.Round(value)) < 0.01f)
            {
                return ((int)Mathf.Round(value)).ToString();
            }
            // 否则显示两位小数
            else
            {
                return value.ToString("F2");
            }
        }

        void OnAttackSkillAcquired(SkillAcquiredEvent e)
        {
            if (e.Model != _playerModel)
            {
                return;
            }

            if (e.Skill is not AttackSkill attackSkill)
            {
                return;
            }

            foreach (IStat stat in attackSkill.SkillStats.GetAllStats())
            {
                stat.Register(UpdateStatsInfo);
            }
        }

        void Awake()
        {
            this.RegisterEvent<SkillAcquiredEvent>(OnAttackSkillAcquired).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        void Start()
        {
            _playerModel = this.GetModel<PlayersModel>().Current;
            foreach (IStat stat in _playerModel.Stats.GetAllStats())
            {
                stat.Register(UpdateStatsInfo);
            }


            UpdateStatsInfo();
        }

        void OnValidate()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }
}
