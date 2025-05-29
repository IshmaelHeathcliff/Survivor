using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Drop;
using SaveLoad;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DropSystem : AbstractSystem
{
    Dictionary<string, DropInfo> _dropInfos = new();
    readonly List<GameObject> _dropPrefabs = new();

    const string JsonPath = "Preset";
    const string JsonName = "Drops.json";

    readonly List<AsyncOperationHandle<GameObject>> _dropPrefabHandles = new();

    protected override void OnInit()
    {
        LoadDrops();
    }

    protected override void OnDeinit()
    {
        foreach (AsyncOperationHandle<GameObject> handle in _dropPrefabHandles)
        {
            AddressablesManager.Release(handle);
        }
    }

    async void LoadDrops()
    {
        var dropInfos = this.GetUtility<SaveLoadUtility>().Load<List<DropInfo>>(JsonName, JsonPath);
        foreach (DropInfo dropInfo in dropInfos)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(dropInfo.DropPrefab);
            _dropPrefabHandles.Add(handle);
            GameObject prefab = await handle;
            _dropPrefabs.Add(prefab);
            _dropInfos.Add(dropInfo.DropID, dropInfo);
        }
    }

    public string GetDropAddress(string dropId)
    {
        if (_dropInfos.TryGetValue(dropId, out DropInfo dropInfo))
        {
            return dropInfo.DropPrefab;
        }

        Debug.LogError($"Drop prefab not found: {dropId}");
        return null;
    }

}
