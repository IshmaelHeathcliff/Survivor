using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Status Preset", fileName = "StatusPreset")]
    public class StatusPreset : DataPreset<StatusConfig>
    {
    }
}
