using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Enemy Preset", fileName = "EnemyPreset")]
    public class EnemyPreset : DataPreset<EnemyConfig>
    {
    }
}
