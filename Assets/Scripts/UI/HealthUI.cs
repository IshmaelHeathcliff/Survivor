using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI
{
    public class HealthUI : MonoBehaviour
    {
        public const int HeartSize = 4;


        [SerializeField] AssetReference _fullHeart;
        [SerializeField] AssetReference _threeQuarterHeart;
        [SerializeField] AssetReference _halfHeart;
        [SerializeField] AssetReference _quarterHeart;

        [SerializeField] AssetReference _fullHeartBackground;
        [SerializeField] AssetReference _threeQuarterHeartBackground;
        [SerializeField] AssetReference _halfHeartBackground;
        [SerializeField] AssetReference _quarterHeartBackground;

        [SerializeField] Image _background;
        [SerializeField] Image _current;

        Sprite _fullHeartSprite;
        Sprite _threeQuarterHeartSprite;
        Sprite _halfHeartSprite;
        Sprite _quarterHeartSprite;

        Sprite _fullHeartBackgroundSprite;
        Sprite _threeQuarterHeartBackgroundSprite;
        Sprite _halfHeartBackgroundSprite;
        Sprite _quarterHeartBackgroundSprite;

        bool _spritesLoaded = false;
        UniTask _loadSpritesTask;

        async UniTask LoadSprites()
        {
            _fullHeartSprite = await _fullHeart.LoadAssetAsync<Sprite>();
            _threeQuarterHeartSprite = await _threeQuarterHeart.LoadAssetAsync<Sprite>();
            _halfHeartSprite = await _halfHeart.LoadAssetAsync<Sprite>();
            _quarterHeartSprite = await _quarterHeart.LoadAssetAsync<Sprite>();

            _fullHeartBackgroundSprite = await _fullHeartBackground.LoadAssetAsync<Sprite>();
            _threeQuarterHeartBackgroundSprite = await _threeQuarterHeartBackground.LoadAssetAsync<Sprite>();
            _halfHeartBackgroundSprite = await _halfHeartBackground.LoadAssetAsync<Sprite>();
            _quarterHeartBackgroundSprite = await _quarterHeartBackground.LoadAssetAsync<Sprite>();

            _spritesLoaded = true;
        }

        public async void SetCurrentHealth(int health)
        {
            if (!_spritesLoaded)
            {
                await _loadSpritesTask;
            }

            switch (health)
            {
                case 4:
                    _current.sprite = _fullHeartSprite;
                    break;
                case 3:
                    _current.sprite = _threeQuarterHeartSprite;
                    break;
                case 2:
                    _current.sprite = _halfHeartSprite;
                    break;
                case 1:
                    _current.sprite = _quarterHeartSprite;
                    break;
                case 0:
                    _current.sprite = _quarterHeartBackgroundSprite;
                    break;
                default:
                    Debug.LogError($"Invalid heart health: {health}");
                    _current.sprite = _quarterHeartBackgroundSprite;
                    break;
            }
        }

        public async void SetMaxHealth(int maxHealth)
        {
            if (!_spritesLoaded)
            {
                await _loadSpritesTask;
            }

            switch (maxHealth)
            {
                case 4:
                    gameObject.SetActive(true);
                    _background.sprite = _fullHeartBackgroundSprite;
                    break;
                case 3:
                    gameObject.SetActive(true);
                    _background.sprite = _threeQuarterHeartBackgroundSprite;
                    break;
                case 2:
                    gameObject.SetActive(true);
                    _background.sprite = _halfHeartBackgroundSprite;
                    break;
                case 1:
                    gameObject.SetActive(true);
                    _background.sprite = _quarterHeartBackgroundSprite;
                    break;
                case 0:
                    gameObject.SetActive(false);
                    break;
                default:
                    Debug.LogError($"Invalid heart max health: {maxHealth}");
                    gameObject.SetActive(false);
                    break;
            }
        }

        void OnValidate()
        {
            _current = transform.Find("Current").GetComponent<Image>();
            _background = GetComponent<Image>();
        }


        async void Awake()
        {
            _loadSpritesTask = LoadSprites();
            await _loadSpritesTask;
        }

        void OnDestroy()
        {
            Addressables.Release(_fullHeartSprite);
            Addressables.Release(_threeQuarterHeartSprite);
            Addressables.Release(_halfHeartSprite);
            Addressables.Release(_quarterHeartSprite);
            Addressables.Release(_fullHeartBackgroundSprite);
            Addressables.Release(_threeQuarterHeartBackgroundSprite);
            Addressables.Release(_halfHeartBackgroundSprite);
            Addressables.Release(_quarterHeartBackgroundSprite);
        }
    }
}
