using System.Collections.Generic;
using XYZRPGSystem.Data.SaveLoad;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;

namespace Editor
{
    public abstract class DataPreset<T> : SerializedScriptableObject
    {
        [HorizontalGroup("Json")]
        public string JsonName = ".json";
        [HorizontalGroup("Json")]
        public string JsonPath = "Preset/JSON";

        [PropertySpace(10)]
        [ButtonGroup]
        [Button("Read")]
        protected virtual void ReadJson()
        {
            Data = SaveLoadManager.Load<List<T>>(JsonName, JsonPath);

        }

        [ButtonGroup]
        [Button("Save")]
        public virtual void SaveToJson()
        {
            SaveLoadManager.Save(Data, JsonName, JsonPath);
        }

        protected virtual void OnEnable()
        {
            ReadJson();
        }

        protected void DrawRefreshButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                ReadJson();
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.File))
            {
                SaveToJson();
            }
        }

        [PropertySpace(10)]
        [OdinSerialize]
        [ListDrawerSettings(
            DraggableItems = true,
            ShowFoldout = false,
            ShowPaging = true,
            ShowIndexLabels = true,
            AddCopiesLastElement = true,
            NumberOfItemsPerPage = 30,
            DefaultExpandedState = true,
            OnTitleBarGUI = "DrawRefreshButton",
            ListElementLabelName = "Name"
            )]
        [TableList(
            ShowIndexLabels = true,
            DrawScrollView = true,
            // MaxScrollViewHeight = 400,
            AlwaysExpanded = true
        )]
        [ShowInInspector]
        public virtual List<T> Data { get; set; }
    }


}
