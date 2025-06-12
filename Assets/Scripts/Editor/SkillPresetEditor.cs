using Data.Config;
using UnityEngine;
using GamePlay.Skill;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Skill Preset Editor", fileName = "SkillPresetEditor")]
    public class SkillPresetEditor : DataPresetEditor<SkillConfig>
    {
    }
}
