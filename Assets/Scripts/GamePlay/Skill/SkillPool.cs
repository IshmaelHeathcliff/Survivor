using System.Collections.Generic;
using System.Linq;
using Data.Config;
using UnityEngine;

namespace GamePlay.Skill
{
    public interface ISkillPool
    {
        bool AddSkill(SkillConfig config);
        bool RemoveSkill(SkillConfig config);
        bool HasSkill(SkillConfig config);
        bool HasSkills(IEnumerable<SkillConfig> configs); // ( config1, config2, ...)
        bool AddSkills(IEnumerable<SkillConfig> configs);
        bool RemoveSkills(IEnumerable<SkillConfig> configs);
        SkillConfig PopRandomSkill(SkillRarity rarity);
        int GetCount(SkillRarity rarity);
        int GetCount();
    }
    public class SkillPool : ISkillPool
    {
        Dictionary<SkillRarity, List<SkillConfig>> SkillsInPool { get; } = new()
        {
            { SkillRarity.Common, new List<SkillConfig>() },
            { SkillRarity.Magic, new List<SkillConfig>() },
            { SkillRarity.Rare, new List<SkillConfig>() },
            { SkillRarity.Epic, new List<SkillConfig>() },
            { SkillRarity.Legendary, new List<SkillConfig>() },
        };

        public bool AddSkill(SkillConfig config)
        {
            if (SkillsInPool[config.Rarity].Contains(config))
            {
                return false;
            }

            SkillsInPool[config.Rarity].Add(config);
            return true;
        }

        public bool RemoveSkill(SkillConfig config)
        {
            return SkillsInPool[config.Rarity].Remove(config);
        }

        public bool HasSkill(SkillConfig config)
        {
            return SkillsInPool[config.Rarity].Contains(config);
        }

        public bool HasSkills(IEnumerable<SkillConfig> configs)
        {
            return configs.All(HasSkill);
        }

        public bool AddSkills(IEnumerable<SkillConfig> configs)
        {
            return configs.All(AddSkill);
        }

        public bool RemoveSkills(IEnumerable<SkillConfig> configs)
        {
            return configs.All(RemoveSkill);
        }

        public SkillConfig PopRandomSkill(SkillRarity rarity)
        {
            SkillConfig config = SkillsInPool[rarity].ElementAt(Random.Range(0, SkillsInPool[rarity].Count));
            RemoveSkill(config);
            return config;
        }

        public IEnumerable<SkillConfig> GetAllSkills()
        {
            return SkillsInPool.SelectMany(kvp => kvp.Value);
        }

        public int GetCount(SkillRarity rarity)
        {
            return SkillsInPool[rarity].Count;
        }

        public int GetCount()
        {
            return SkillsInPool.SelectMany(kvp => kvp.Value).Count();
        }
    }

    public abstract class SkillPoolAddRule
    {
        public string Name { get; set; }
        public List<string> SkillIDsToAdd { get; set; }
    }

    public abstract class SkillPoolAddRule<T> : SkillPoolAddRule where T : SkillPoolAddRuleConfig
    {
        protected T Config { get; set; }

        protected SkillPoolAddRule(T config)
        {
            Name = config.Name;
            Config = config;
            SkillIDsToAdd = config.SkillIDsToAdd;
        }
    }

    public class SpecificSkillsPoolAddRule : SkillPoolAddRule<SpecificSkillsPoolAddRuleConfig>
    {
        public List<string> RequiredSkillIDs { get; set; }
        public SpecificSkillsPoolAddRule(SpecificSkillsPoolAddRuleConfig config) : base(config)
        {
            RequiredSkillIDs = config.RequiredSkillIDs;
        }
    }
}
