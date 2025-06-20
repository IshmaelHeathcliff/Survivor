using System.Collections.Generic;
using System.Linq;
using Data.Config;
using Data.SaveLoad;
using GamePlay.Character.Modifier;
using UnityEngine;

namespace GamePlay.Character.State
{
    public class StateCreateSystem : AbstractSystem
    {
        Dictionary<string, StateConfig> _stateConfigCache;
        const string JsonPath = "Preset";
        const string JsonName = "States.json";

        void Load()
        {
            _stateConfigCache = new Dictionary<string, StateConfig>();
            List<StateConfig> stateConfigList = this.GetUtility<SaveLoadUtility>().Load<List<StateConfig>>(JsonName, JsonPath);
            foreach (StateConfig stateConfig in stateConfigList)
            {
                _stateConfigCache.Add(stateConfig.ID, stateConfig);
            }
        }

        public StateConfig GetStateConfig(string id)
        {
            if (_stateConfigCache == null)
            {
                Load();
            }

            if (_stateConfigCache.TryGetValue(id, out StateConfig config))
            {
                return config;
            }

            Debug.LogError($"StateConfig not found: {id}");
            return null;
        }

        public IState CreateState(string id, string factoryID, List<int> values = null)
        {
            IStatModifierFactory factory = this.GetSystem<ModifierSystem>().GetModifierFactory<IStatModifierFactory>(factoryID);
            return CreateState(id, factory, values);
        }

        public IStateWithTime CreateState(string id, string factoryID, int time = -1, List<int> values = null)
        {
            IStatModifierFactory factory = this.GetSystem<ModifierSystem>().GetModifierFactory<IStatModifierFactory>(factoryID);
            return CreateState(id, factory, time, values);
        }


        public IState CreateState(string id, IStatModifierFactory factory, List<int> values = null)
        {
            StateConfig stateConfig = GetStateConfig(id);
            ModifierSystem modifierSystem = this.GetSystem<ModifierSystem>();
            if (values != null && stateConfig.ModifierEntries.Count != values.Count)
            {
                Debug.LogError("values.Length != stateConfig.EntryID.Count");
                return null;
            }

            IEnumerable<IStatModifier> entries = stateConfig.ModifierEntries.Select(
                    (entry, i) => modifierSystem.CreateStatModifier(entry.ModifierID, factory, values != null ? values[i] : entry.Value));

            return new State(stateConfig, entries);
        }

        public IStateWithTime CreateState(string id, IStatModifierFactory factory, int time = -1, List<int> values = null)
        {
            StateConfig stateConfig = GetStateConfig(id);

            ModifierSystem modifierSystem = this.GetSystem<ModifierSystem>();

            if (values != null && stateConfig.ModifierEntries.Count != values.Count)
            {
                Debug.LogError("values.Length != stateConfig.EntryID.Count");
                return null;
            }

            var entries = new List<IModifier>();

            for (int i = 0; i < stateConfig.ModifierEntries.Count; i++)
            {
                entries.Add(modifierSystem.CreateStatModifier(stateConfig.ModifierEntries[i].ModifierID, factory, values != null ? values[i] : stateConfig.ModifierEntries[i].Value));
            }

            return new StateWithTime(stateConfig, entries, time == -1 ? stateConfig.Duration : time);
        }



        protected override void OnInit()
        {
            _stateConfigCache = new Dictionary<string, StateConfig>();
            Load();
        }
    }
}
