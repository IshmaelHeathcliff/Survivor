using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Enemy Spawn Group Preset", fileName = "EnemySpawnGroupPreset")]
    public class EnemySpawnGroupPreset : DataPreset<EnemySpawnGroupConfig>
    {
    }
}
