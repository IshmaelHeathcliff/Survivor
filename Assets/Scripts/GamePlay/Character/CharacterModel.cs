using System.Collections.Generic;
using Character.State;
using Character.Stat;
using UnityEngine;
using System;

namespace Character
{
    public interface ICharacterModel
    {
        float Speed { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        Stats Stats { get; }
        IStateContainer StateContainer { get; }
    }

    public abstract class CharacterModel : ICharacterModel
    {
        Transform _transform;
        public float Speed { get; set; }

        public Vector2 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Vector2 Direction { get; set; }
        public Stats Stats { get; } = new Stats();
        public IStateContainer StateContainer { get; } = new StateContainer();

        public CharacterModel(Transform transform)
        {
            _transform = transform;
        }

    }

    public abstract class CharactersModel<T> : AbstractModel where T : ICharacterModel
    {
        readonly protected Dictionary<string, T> Models = new();


        public bool TryGetModel(string id, out T model)
        {
            if (Models.TryGetValue(id, out model))
            {
                return true;
            }

            return false;
        }

        public void AddModel(string id, T model)
        {
            Models.Add(id, model);
        }

        public void RemoveModel(string id)
        {
            Models.Remove(id);
        }

        // TODO: 获取当前Model
        public abstract T Current();
    }
}
