using System.Collections.Generic;
using Data.Config;
using Data.SaveLoad;

namespace GamePlay.Character.Stat
{
    public class SkillStats : Stats
    {
        public SkillStats(List<string> keywords, CharacterStats characterStats)
        {
            List<StatConfig> skillStats = SaveLoadManager.Load<List<StatConfig>>("SkillStats.json", "Preset");
            foreach (StatConfig stat in skillStats)
            {
                switch (stat.Type)
                {
                    case StatType.Consumable:
                        InternalStats.Add(stat.ID, new LocalConsumableStat(new ConsumableStat(stat.ID, stat.Name), characterStats.GetConsumableStat(stat.ID)));
                        break;
                    case StatType.Keyword:
                        InternalStats.Add(stat.ID, new LocalKeywordStat(keywords, new KeywordStat(stat.ID, stat.Name), characterStats.GetKeywordStat(stat.ID)));
                        break;
                    default:
                        InternalStats.Add(stat.ID, new LocalStat(new Stat(stat.ID, stat.Name), characterStats.GetStat(stat.ID)));
                        break;
                }
            }
        }
    }
}


