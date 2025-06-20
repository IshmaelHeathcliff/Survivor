using Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/SkillPoolAddRule Preset Editor", fileName = "SkillPoolAddRulePresetEditor")]
    public class SkillPoolRulePresetEditor : DataPresetEditor<SkillPoolAddRuleConfig>
    {

    }

    [CreateAssetMenu(menuName = "Tools/SkillPoolRemoveRule Preset Editor", fileName = "SkillPoolRemoveRulePresetEditor")]
    public class SkillPoolRemoveRulePresetEditor : DataPresetEditor<SkillPoolRemoveRuleConfig>
    {

    }
}
