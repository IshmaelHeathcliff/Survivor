using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Skill Preset", fileName = "SkillPreset")]
    public class SkillPreset : DataPreset<SkillConfig>
    {
    }
}
