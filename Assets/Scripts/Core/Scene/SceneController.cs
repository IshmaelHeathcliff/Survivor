﻿using Character.Modifier;
using Character.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scene
{
    public class SceneController : MonoBehaviour, IController
    {
        SceneModel _model;

        async UniTaskVoid LoadScene(string sceneName, string entranceTag = null)
        {
            await Transition(sceneName, entranceTag);
        }

        async UniTask Transition(string sceneName, string entranceTag)
        {
            _model.ClearEntrances();

            this.GetSystem<ModifierSystem>().ClearModifierFactories();

            await Addressables.LoadSceneAsync(sceneName);

            this.GetModel<PlayersModel>().Default().Position = _model.GetEntrance(entranceTag).transform.position;
        }

        void Awake()
        {
            _model = this.GetModel<SceneModel>();
        }

        void Start()
        {
            TypeEventSystem.Global.Register<LoadSceneEvent>(e =>
            {
                LoadScene(e.SceneName, e.EntranceTag).Forget();
            });
        }

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }
    }

    public struct LoadSceneEvent
    {
        public string SceneName;
        public string EntranceTag;
    }

}
