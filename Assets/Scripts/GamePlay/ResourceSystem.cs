using System;
using System.Collections.Generic;
using Character.Player;
using UnityEngine;

public class ResourceSystem : AbstractSystem
{
    Dictionary<string, BindableProperty<int>> Resources => this.GetModel<PlayersModel>().Current.Resources;

    public void RegisterResource(string id, BindableProperty<int> resource, int initialValue = 0)
    {
        Resources.Add(id, resource);
        resource.Value = initialValue;
    }

    public void UnregisterResource(string id)
    {
        Resources.Remove(id);
    }

    public BindableProperty<int> GetResource(string id)
    {
        if (!Resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return null;
        }

        return Resources[id];
    }

    public IUnRegister Register(string id, Action<int> onValueChanged)
    {
        if (!Resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return null;
        }

        return Resources[id].Register(onValueChanged);
    }

    public void Unregister(string id, Action<int> onValueChanged)
    {
        if (!Resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }

        Resources[id].UnRegister(onValueChanged);
    }


    public void AcquireResource(string id, int amount)
    {
        if (!Resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }
        Resources[id].Value += amount;
    }

    public void ConsumeResource(string id, int amount)
    {
        if (!Resources.ContainsKey(id))
        {
            Debug.LogError($"Resource {id} not found");
            return;
        }
        Resources[id].Value -= amount;
    }

    protected override void OnInit()
    {
    }
}