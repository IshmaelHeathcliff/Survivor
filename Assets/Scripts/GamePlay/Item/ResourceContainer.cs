using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GamePlay.Item
{
    public interface IResourceContainer
    {
        int Coin { get; set; }
        int Wood { get; set; }

        int GetResourceCount(string id);
        void SetResourceCount(string id, int value);
        void AddResourceCount(string id, int value);

        IUnRegister Register(string id, Action<int> onValueChanged);
        IUnRegister RegisterWithInitValue(string id, Action<int> onValueChanged);
        void UnRegister(string id, Action<int> onValueChanged);
        IUnRegister Register(ResourceType type, Action<int> onValueChanged);
        IUnRegister RegisterWithInitValue(ResourceType type, Action<int> onValueChanged);
        void UnRegister(ResourceType type, Action<int> onValueChanged);
        IUnRegister RegisterCoin(Action<int> onValueChanged);
        IUnRegister RegisterWood(Action<int> onValueChanged);
        void UnRegisterCoin(Action<int> onValueChanged);
        void UnRegisterWood(Action<int> onValueChanged);
    }

    public enum ResourceType
    {
        Coin,
        Wood,
    }

    public class ResourceContainer : IResourceContainer
    {
        public int Coin
        {
            get => Resources[ResourceType.Coin].Value;
            set => Resources[ResourceType.Coin].Value = value;
        }

        public int Wood
        {
            get => Resources[ResourceType.Wood].Value;
            set => Resources[ResourceType.Wood].Value = value;
        }

        Dictionary<string, ResourceType> ResourceTypeMap { get; } = new();
        Dictionary<ResourceType, BindableProperty<int>> Resources { get; } = new();

        public ResourceContainer()
        {
            ResourceTypeMap.Add("Coin", ResourceType.Coin);
            ResourceTypeMap.Add("Wood", ResourceType.Wood);

            foreach (ResourceType type in ResourceTypeMap.Values)
            {
                Resources.Add(type, new BindableProperty<int>());
            }
        }

        public int GetResourceCount(string id)
        {
            if (ResourceTypeMap.TryGetValue(id, out ResourceType type))
            {
                return Resources[type].Value;
            }

            Debug.LogError($"Resource type not found: {id}");
            return 0;
        }

        public void SetResourceCount(string id, int value)
        {
            if (ResourceTypeMap.TryGetValue(id, out ResourceType type))
            {
                Resources[type].Value = value;
            }
            else
            {
                Debug.LogError($"Resource type not found: {id}");
            }

        }

        public void AddResourceCount(string id, int value)
        {
            if (ResourceTypeMap.TryGetValue(id, out ResourceType type))
            {
                Resources[type].Value += value;
            }
            else
            {
                Debug.LogError($"Resource type not found: {id}");
            }
        }

        public IUnRegister Register(string id, Action<int> onValueChanged)
        {
            return Resources[ResourceTypeMap[id]].Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(string id, Action<int> onValueChanged)
        {
            return Resources[ResourceTypeMap[id]].RegisterWithInitValue(onValueChanged);
        }

        public void UnRegister(string id, Action<int> onValueChanged)
        {
            Resources[ResourceTypeMap[id]].UnRegister(onValueChanged);
        }

        public IUnRegister Register(ResourceType type, Action<int> onValueChanged)
        {
            return Resources[type].Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(ResourceType type, Action<int> onValueChanged)
        {
            return Resources[type].RegisterWithInitValue(onValueChanged);
        }

        public void UnRegister(ResourceType type, Action<int> onValueChanged)
        {
            Resources[type].UnRegister(onValueChanged);
        }

        public IUnRegister RegisterCoin(Action<int> onValueChanged)
        {
            return Register(ResourceType.Coin, onValueChanged);
        }

        public IUnRegister RegisterWood(Action<int> onValueChanged)
        {
            return Register(ResourceType.Wood, onValueChanged);
        }

        public void UnRegisterCoin(Action<int> onValueChanged)
        {
            UnRegister(ResourceType.Coin, onValueChanged);
        }

        public void UnRegisterWood(Action<int> onValueChanged)
        {
            UnRegister(ResourceType.Wood, onValueChanged); ;
        }
    }
}
