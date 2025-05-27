using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamePlay.Drop;
using SaveLoad;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DropSystem : AbstractSystem
{
    readonly Dictionary<string, GameObject> _dropPrefabs = new();

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
        List<DropInfo> dropInfoList = this.GetUtility<SaveLoadUtility>().Load<List<DropInfo>>(JsonName, JsonPath);
        foreach (DropInfo dropInfo in dropInfoList)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(dropInfo.DropPrefab);
            _dropPrefabHandles.Add(handle);
            GameObject prefab = await handle;
            _dropPrefabs.Add(dropInfo.DropID, prefab);
        }
    }

    public GameObject GetDrop(string dropId)
    {
        if (_dropPrefabs.TryGetValue(dropId, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogError($"Drop prefab not found: {dropId}");
        return null;
    }

}
