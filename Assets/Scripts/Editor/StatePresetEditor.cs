using Data.Config;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/State Preset Editor", fileName = "StatePresetEditor")]
    public class StatePresetEditor : DataPresetEditor<StateConfig>
    {
    }
}
