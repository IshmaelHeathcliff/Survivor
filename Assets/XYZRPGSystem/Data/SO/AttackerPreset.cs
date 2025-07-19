using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Attacker Preset", fileName = "AttackerPreset")]
    public class AttackerPreset : DataPreset<AttackerConfig>
    {
    }
}
