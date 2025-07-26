using System.Collections.Generic;
using System.Linq;
using XYZRPGSystem.Data.Config;
using XYZRPGSystem.Data.SaveLoad;

namespace XYZRPGSystem.Gameplay.Stat
{
    public class CharacterStats : Stats
    {
        public CharacterStats()
        {
            LoadStats("CharacterStats.json", "Preset/JSON");
            LoadStats("SkillStats.json", "Preset/JSON");
        }
    }
}
