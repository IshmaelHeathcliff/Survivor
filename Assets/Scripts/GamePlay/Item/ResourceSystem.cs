using System;
using GamePlay.Character;
using GamePlay.Character.Player;
using UnityEngine;

namespace GamePlay.Item
{
    public class ResourceSystem : AbstractSystem
    {

        // 提供默认值，如果model为null，则使用当前玩家模型
        IHasResources CheckModel(IHasResources model)
        {
            if (model == null)
            {
                Debug.LogError("ResourceSystem: model is null, using current player model");
                return this.GetModel<PlayersModel>().Current;
            }

            return model;
        }

        public int GetResourceCount(string id, IHasResources model = null)
        {
            return CheckModel(model).Resources.GetResourceCount(id);
        }

        public IUnRegister Register(string id, Action<int> onValueChanged, IHasResources model = null)
        {
            return CheckModel(model).Resources.Register(id, onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(string id, Action<int> onValueChanged, IHasResources model = null)
        {
            return CheckModel(model).Resources.RegisterWithInitValue(id, onValueChanged);
        }

        public void Unregister(string id, Action<int> onValueChanged, IHasResources model = null)
        {
            CheckModel(model).Resources.UnRegister(id, onValueChanged);
        }

        public void AcquireResource(string id, int amount, IHasResources model = null)
        {
            CheckModel(model).Resources.AddResourceCount(id, amount);
        }

        public void ConsumeResource(string id, int amount, IHasResources model = null)
        {
            CheckModel(model).Resources.AddResourceCount(id, -amount);
        }

        protected override void OnInit()
        {
        }
    }
}
