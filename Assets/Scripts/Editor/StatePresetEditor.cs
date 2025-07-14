using Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Status Preset Editor", fileName = "StatePresetEditor")]
    public class StatePresetEditor : DataPresetEditor<StatusConfig>
    {
    }
}
