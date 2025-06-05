using System.Collections.Generic;
using System.Linq;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

namespace Character.State
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

            return _stateConfigCache.GetValueOrDefault(id);
        }

        public IState CreateState(string id, string factoryID, int[] values)
        {
            StateConfig stateConfig = GetStateConfig(id);
            ModifierSystem entrySystem = this.GetSystem<ModifierSystem>();
            IEnumerable<IStatModifier> entries = stateConfig.ModifierID.Select(
                    (entryId, i) => entrySystem.CreateStatModifier(entryId, factoryID, values[i]));

            return new State(stateConfig, entries);
        }

        public IStateWithTime CreateState(string id, string factoryID, int[] values, int time)
        {
            StateConfig stateConfig = GetStateConfig(id);
            ModifierSystem entrySystem = this.GetSystem<ModifierSystem>();

            if (stateConfig.ModifierID.Count != values.Length)
            {
                Debug.LogError("values.Length != stateConfig.EntryID.Count");
                return null;
            }

            var entries = new List<IModifier>();

            for (int i = 0; i < stateConfig.ModifierID.Count; i++)
            {
                entries.Add(entrySystem.CreateStatModifier(stateConfig.ModifierID[i], factoryID, values[i]));
            }

            return new StateWithTime(stateConfig, entries, time);
        }



        protected override void OnInit()
        {
            _stateConfigCache = new Dictionary<string, StateConfig>();
            Load();
        }
    }
}
