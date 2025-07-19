using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Modifier Preset", fileName = "ModifierPreset")]
    public class ModifierPreset : DataPreset<ModifierConfig>
    {
    }
}
