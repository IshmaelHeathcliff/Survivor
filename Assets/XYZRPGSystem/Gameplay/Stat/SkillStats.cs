using System.Collections.Generic;
using System.Text;
using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Data.SaveLoad;
using XYZRPGSystem.Gameplay.Skill;

namespace XYZRPGSystem.Gameplay.Stat
{
    public class SkillStats : Stats
    {
        public SkillStats(List<string> keywords, CharacterStats characterStats)
        {
            List<StatConfig> skillStatConfigs = SaveLoadManager.Load<List<StatConfig>>("SkillStats.json", "Preset/JSON");
            foreach (StatConfig statConfig in skillStatConfigs)
            {
                switch (statConfig.Type)
                {
                    case StatType.Consumable:
                        InternalStats.Add(statConfig.ID, new LocalConsumableStat(new ConsumableStat(statConfig.ID, statConfig.Name), characterStats.GetConsumableStat(statConfig.ID)));
                        break;
                    case StatType.Keyword:
                        InternalStats.Add(statConfig.ID, new LocalKeywordStat(keywords, new KeywordStat(statConfig.ID, statConfig.Name), characterStats.GetKeywordStat(statConfig.ID)));
                        break;
                    default:
                        InternalStats.Add(statConfig.ID, new LocalStat(new Stat(statConfig.ID, statConfig.Name), characterStats.GetStat(statConfig.ID)));
                        break;
                }
            }
        }

        public static StringBuilder GenerateSkillStatInfo(AttackSkill skill)
        {
            var info = new StringBuilder();
            info.Append($"{skill.Name}: \n");
            info.Append($"  Cooldown: {FormatStatValue(skill.Cooldown)}\n");
            foreach (IStat stat in skill.SkillStats.GetAllStats())
            {
                info.Append(GenerateStatInfo(stat, 1));
            }
            return info;
        }
    }
}


