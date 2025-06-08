using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : AbstractSystem
{
    Dictionary<string, BindableProperty<int>> _resources = new();

    public void RegisterResource(string id, BindableProperty<int> resource, int initialValue = 0)
    {
        _resources.Add(id, resource);
        resource.Value = initialValue;
    }

    public void UnregisterResource(string id)
    {
        _resources.Remove(id);
    }

    public BindableProperty<int> GetResource(string id)
    {
        if (!_resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return null;
        }

        return _resources[id];
    }

    public IUnRegister Register(string id, Action<int> onValueChanged)
    {
        if (!_resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return null;
        }

        return _resources[id].Register(onValueChanged);
    }

    public void Unregister(string id, Action<int> onValueChanged)
    {
        if (!_resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }

        _resources[id].UnRegister(onValueChanged);
    }


    public void AcquireResource(string id, int amount)
    {
        if (!_resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }
        _resources[id].Value += amount;
    }

    public void ConsumeResource(string id, int amount)
    {
        if (!_resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }
        _resources[id].Value -= amount;
    }

    protected override void OnInit()
    {
    }
}