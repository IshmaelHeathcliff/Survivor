using System.Collections.Generic;
using Character.State;
using Character.Stat;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace Character
{
    public interface ICharacterModel
    {
        string ID { get; set; }
        float Speed { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        Stats Stats { get; }
        int SkillSlotCount { get; set; }
        IStateContainer StateContainer { get; }
        ISkillContainer SkillsReleased { get; }
        ISkillContainer SkillsInSlot { get; }

        void BindTransform(Transform transform);
    }

    public abstract class CharacterModel : ICharacterModel
    {
        Transform _transform;
        public string ID { get; set; }
        public float Speed { get; set; }

        public Vector2 Position
        {
            get
            {
                if (_transform == null)
                {
                    return Vector2.zero;
                }

                return _transform.position;
            }
            set
            {
                if (_transform == null)
                {
                    return;
                }

                _transform.position = value;
            }
        }

        public Vector2 Direction { get; set; }
        public Stats Stats { get; } = new Stats();
        public IStateContainer StateContainer { get; } = new StateContainer();

        public int SkillSlotCount
        {
            get => SkillsInSlot.MaxCount;
            set => SkillsInSlot.MaxCount = value;
        }

        public ISkillContainer SkillsReleased { get; } = new SkillContainer();
        public ISkillContainer SkillsInSlot { get; } = new SkillContainer(7);

        public void BindTransform(Transform transform)
        {
            _transform = transform;
        }
    }

    public abstract class CharactersModel<TModel> : AbstractModel where TModel : ICharacterModel, new()
    {
        string _currentID;

        //* 实时调用Current是更安全的
        public TModel Current
        {
            get
            {
                if (TryGetModel(_currentID, out TModel model))
                {
                    return model;
                }

                Debug.LogError($"Current model {_currentID} does not exist;");
                return default;
            }

            set => _currentID = value.ID;
        }

        readonly protected Dictionary<string, TModel> Models = new();
        public bool TryGetModel(string id, out TModel model)
        {
            if (Models.TryGetValue(id, out model))
            {
                return true;
            }

            return false;
        }

        public TModel AddModel(string id)
        {
            if (!Models.TryGetValue(id, out TModel model))
            {
                model = new TModel
                {
                    ID = id
                };

                Models.Add(id, model);
            }

            return model;
        }

        public void RemoveModel(string id)
        {
            Models.Remove(id);
        }
    }
}
