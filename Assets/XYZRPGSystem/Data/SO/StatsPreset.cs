using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Stats Preset", fileName = "StatsPreset")]
    public class StatsPreset : DataPreset<StatConfig>
    {
    }
}
