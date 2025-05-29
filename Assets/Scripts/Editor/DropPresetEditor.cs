using System.Collections.Generic;
using GamePlay.Drop;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor
{
    [CreateAssetMenu(menuName = "Tools/Drop Preset Editor", fileName = "DropPresetEditor")]
    public class DropPresetEditor : DataPresetEditor<DropInfo>
    {
    }
}
