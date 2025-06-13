using System;
using System.Collections.Generic;
using System.Linq;
using Data.Config;
using Data.SaveLoad;
using GamePlay.Character;
using GamePlay.Character.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Skill
{


    public class SkillGachaSystem : AbstractSystem
    {
        const string PresetPath = "Preset";
        const string AddRulesPath = "SkillPoolAddRules.json";

        SkillSystem _skillSystem;
        List<SkillPoolAddRule> _skillAddRules = new();
        SkillPoolAddRuleLoader _skillPoolAddRuleLoader = new();

        readonly List<IUnRegister> _unRegisters = new();

        Dictionary<SkillRarity, int> _skillGachaWeights = new()
        {
            { SkillRarity.Common, 50 },
            { SkillRarity.Magic, 30 },
            { SkillRarity.Rare, 15 },
            { SkillRarity.Epic, 4 },
            { SkillRarity.Legendary, 1},
        };

        void Load()
        {
            _skillAddRules.Clear();
            List<SkillPoolAddRuleConfig> addRuleConfigs = this.GetUtility<SaveLoadUtility>().Load<List<SkillPoolAddRuleConfig>>(AddRulesPath, PresetPath);
            foreach (SkillPoolAddRuleConfig config in addRuleConfigs)
            {
                _skillAddRules.Add(_skillPoolAddRuleLoader.Load(config));
            }

            foreach (SkillPoolAddRuleConfig config in addRuleConfigs)
            {
                Debug.Log($"卡池规则 {config.Name} 已添加");
            }
        }

        public void InitSkillPool(ICharacterModel model, string path)
        {
            List<SkillInPool> initSkills = this.GetUtility<SaveLoadUtility>().Load<List<SkillInPool>>(path, PresetPath);
            foreach (SkillInPool skill in initSkills)
            {
                SkillConfig config = _skillSystem.GetSkillConfig(skill.Name);
                if (config != null)
                {
                    model.SkillPool.AddSkill(config);
                }
            }
        }

        public List<SkillConfig> GachaSkills(ICharacterModel model, int count = 1)
        {
            if (model.SkillPool.GetCount() < count)
            {
                count = model.SkillPool.GetCount();
            }

            int totalWeight = 0;
            Dictionary<SkillRarity, int> effectiveWeights = new();
            foreach ((SkillRarity rarity, int weight) in _skillGachaWeights)
            {
                if (model.SkillPool.GetCount(rarity) > 0)
                {
                    totalWeight += weight;
                    effectiveWeights[rarity] = weight;
                }
            }

            List<SkillConfig> result = new();
            for (int i = 0; i < count; i++)
            {
                int randomValue = Random.Range(0, totalWeight);
                foreach ((SkillRarity rarity, int weight) in effectiveWeights)
                {
                    if (randomValue < weight)
                    {
                        SkillConfig config = model.SkillPool.PopRandomSkill(rarity);
                        result.Add(config);
                    }

                    randomValue -= weight;
                }
            }

            this.SendEvent(new GachaSkillsEvent(result, model));

            return result;
        }

        public void SelectSkill(ICharacterModel model, List<SkillConfig> configs, int index)
        {
            for (int i = 0; i < configs.Count; i++)
            {
                if (i != index)
                {
                    model.SkillPool.AddSkill(configs[i]);
                }
            }

            _skillSystem.AcquireSkill(configs[index].ID, model);

            this.SendEvent(new SelectSkillEvent(configs, index, model));
        }

        public void CancelSelect(ICharacterModel model, List<SkillConfig> configs)
        {
            foreach (SkillConfig config in configs)
            {
                model.SkillPool.AddSkill(config);
            }
        }


        void RegisterRules()
        {
            _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(OnSkillAcquired));
            _unRegisters.Add(this.RegisterEvent<SkillRemovedEvent>(OnSkillRemoved));

            foreach (SkillPoolAddRule rule in _skillAddRules)
            {
                if (rule is SpecificSkillsPoolAddRule specificAddRule)
                {
                    RegisterSpecificSkillsPoolAddRule(specificAddRule);
                }
            }
        }

        void RegisterSpecificSkillsPoolAddRule(SpecificSkillsPoolAddRule rule)
        {
            _unRegisters.Add(this.RegisterEvent<SkillAcquiredEvent>(e =>
            {
                if (!rule.RequiredSkillIDs.Contains(e.Skill.ID))
                {
                    return;
                }

                if (e.Model.HasSkills(rule.RequiredSkillIDs))
                {
                    var skillConfigsToAdd = _skillSystem.GetSkillConfigs(rule.SkillIDsToAdd).ToList();
                    e.Model.SkillPool.AddSkills(skillConfigsToAdd);
                    foreach (SkillConfig config in skillConfigsToAdd)
                    {
                        Debug.Log($"Skill {config.ID} added to pool");
                    }
                }
            })
            );
            Debug.Log($"卡池规则 {rule.Name} 已注册");

        }

        void OnSkillAcquired(SkillAcquiredEvent e)
        {
            SkillConfig skillConfig = _skillSystem.GetSkillConfig(e.Skill.ID);
            e.Model.SkillPool.RemoveSkill(skillConfig);
        }

        void OnSkillRemoved(SkillRemovedEvent e)
        {
            SkillConfig skillConfig = _skillSystem.GetSkillConfig(e.SkillID);
            e.Model.SkillPool.AddSkill(skillConfig);
        }

        protected override void OnInit()
        {
            _skillSystem = this.GetSystem<SkillSystem>();

            Load();
            RegisterRules();
        }

        protected override void OnDeinit()
        {
            base.OnDeinit();
            foreach (IUnRegister unRegister in _unRegisters)
            {
                unRegister.UnRegister();
            }
        }
    }

    public class SkillPoolAddRuleLoader
    {
        public SkillPoolAddRule Load(SkillPoolAddRuleConfig config)
        {
            return config switch
            {
                SpecificSkillsPoolAddRuleConfig specificConfig => new SpecificSkillsPoolAddRule(specificConfig),
                _ => null
            };
        }
    }
}
