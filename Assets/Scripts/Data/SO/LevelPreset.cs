using Data.Config;
using UnityEngine;
using XYZRPGSystem.Data.SO;

namespace Data.SO
{
    [CreateAssetMenu(menuName = "Preset/Level Preset", fileName = "LevelPreset")]
    public class LevelPreset : DataPreset<LevelConfig>
    {
    }
}
