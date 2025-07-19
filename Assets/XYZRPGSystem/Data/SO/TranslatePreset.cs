using XYZRPGSystem.Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Preset/Translate Preset", fileName = "TranslatePreset")]
    public class TranslatePreset : DataPreset<TranslateEntryConfig>
    {
    }
}
