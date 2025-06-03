using System.Collections.Generic;
using Character.Stat;
using SaveLoad;
using UnityEngine;

namespace Character.Modifier
{
    public class ModifierSystem : AbstractSystem
    {
        Dictionary<string, ModifierInfo> _modifierInfoCache = new();
        const string JsonPath = "Preset";
        const string JsonName = "Modifiers.json";

        void Load()
        {
            _modifierInfoCache = new Dictionary<string, ModifierInfo>();
            List<ModifierInfo> modifierInfoList = this.GetUtility<SaveLoadUtility>().Load<List<ModifierInfo>>(JsonName, JsonPath);
            foreach (ModifierInfo modifierInfo in modifierInfoList)
            {
                _modifierInfoCache.Add(modifierInfo.ModifierID, modifierInfo);
            }
        }

        public ModifierInfo GetModifierInfo(string id)
        {
            if (_modifierInfoCache == null)
            {
                Load();
            }

            if (!_modifierInfoCache.TryGetValue(id, out ModifierInfo modifierInfo))
            {
                Debug.LogError($"modifierID {id} is not registered");
                return null;
            }

            return modifierInfo;
        }

        public T GetModifierInfo<T>(string id) where T : ModifierInfo
        {
            ModifierInfo modifierInfo = GetModifierInfo(id);
            if (modifierInfo == null)
            {
                return null;
            }

            if (modifierInfo is not T t)
            {
                Debug.LogError($"modifierID {id} is not a {typeof(T).Name}");
                return null;
            }

            return t;
        }

        readonly Dictionary<string, IModifierFactory> _modifierFactories = new();

        public void ClearModifierFactories()
        {
            _modifierFactories.Clear();
        }

        public void RegisterFactory(IModifierFactory factory)
        {
            _modifierFactories.Add(factory.FactoryID, factory);
        }

        public void UnregisterFactory(string factoryID)
        {
            _modifierFactories.Remove(factoryID);
        }

        public IStat GetStat(IStatModifier modifier)
        {
            var factory = GetModifierFactory<IStatModifierFactory>(modifier.FactoryID);
            if (factory == null)
            {
                return null;
            }

            return factory.GetStat(modifier.ModifierInfo);
        }

        public IModifierFactory GetModifierFactory(string factoryID)
        {
            if (!_modifierFactories.TryGetValue(factoryID, out IModifierFactory factory))
            {
                Debug.LogError($"factoryID {factoryID} is not registered");
                return null;
            }

            return factory;
        }

        public T GetModifierFactory<T>(string factoryID) where T : IModifierFactory
        {
            IModifierFactory factory = GetModifierFactory(factoryID);
            if (factory == null)
            {
                return default;
            }

            if (factory is not T t)
            {
                Debug.LogError($"factoryID {factoryID} is not a {typeof(T).Name}");
                return default;
            }

            return t;
        }


        public IStatModifier CreateStatModifier(string modifierId, string factoryID)
        {
            IStatModifierFactory factory = GetModifierFactory<IStatModifierFactory>(factoryID);
            if (factory == null)
            {
                return null;
            }

            return CreateStatModifier(modifierId, factory);
        }

        public IStatModifier CreateStatModifier(string modifierId, IStatModifierFactory factory)
        {
            StatModifierInfo modifierInfo = GetModifierInfo<StatModifierInfo>(modifierId);
            if (modifierInfo == null)
            {
                return null;
            }

            return factory.CreateModifier(modifierInfo);
        }

        public IStatModifier CreateStatModifier(string modifierId, string factoryID, int value)
        {

            IStatModifierFactory factory = GetModifierFactory<IStatModifierFactory>(factoryID);
            if (factory == null)
            {
                return null;
            }

            return CreateStatModifier(modifierId, factory, value);
        }

        public IStatModifier CreateStatModifier(string modifierId, IStatModifierFactory factory, int value)
        {
            StatModifierInfo modifierInfo = GetModifierInfo<StatModifierInfo>(modifierId);
            if (modifierInfo == null)
            {
                return null;
            }

            return factory.CreateModifier(modifierInfo, value);
        }

        protected override void OnInit()
        {
            Load();
        }
    }
}
