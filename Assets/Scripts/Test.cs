using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] AssetReferenceGameObject _prefabReference;
    [SerializeField] AssetReferenceTexture2D _coinReference;
    [SerializeField] AssetReferenceTexture2D _heartReference;
    GameObject _instance;
    Texture2D _heart;
    Texture2D _coin;

    void Start()
    {
    }

    [Button]
    void LoadPrefab()
    {
        _instance = Instantiate(_prefab);
    }


    [Button]
    async UniTask LoadPrefabAsync()
    {
        _instance = await Addressables.InstantiateAsync(_prefabReference);
    }

    [Button]
    async UniTask LoadHeartAsync()
    {
        _heart = await Addressables.LoadAssetAsync<Texture2D>(_heartReference);
        _instance.GetComponent<SpriteRenderer>().sprite = Sprite.Create(_heart, new Rect(0, 0, _heart.width, _heart.height), Vector2.zero);
    }

    [Button]
    async UniTask LoadCoinAsync()
    {
        _coin = await Addressables.LoadAssetAsync<Texture2D>(_coinReference);
        _instance.GetComponent<SpriteRenderer>().sprite = Sprite.Create(_coin, new Rect(0, 0, _coin.width, _coin.height), Vector2.zero);
    }

    [Button]
    void ReleasePrefab()
    {
        Addressables.ReleaseInstance(_instance);
    }

    [Button]
    void ReleaseHeart()
    {
        Addressables.Release(_heart);
    }

    [Button]
    void ReleaseCoin()
    {
        Addressables.Release(_coin);
    }
}
