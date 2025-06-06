using System.Collections.Generic;
using Character.State;
using Character.Stat;
using UnityEngine;
using System;

namespace Character
{
    public interface ICharacterModel
    {
        string ID { get; set; }
        float Speed { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        Stats Stats { get; }
        IStateContainer StateContainer { get; }
        ISkillContainer SkillsReleased { get; }
        ISkillContainer SkillsInSlot { get; }
    }

    public abstract class CharacterModel : ICharacterModel
    {
        Transform _transform;
        public string ID { get; set; }
        public float Speed { get; set; }

        public Vector2 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Vector2 Direction { get; set; }
        public Stats Stats { get; } = new Stats();
        public IStateContainer StateContainer { get; } = new StateContainer();

        public int SkillSlotCount { get; set; } = 7;
        public ISkillContainer SkillsReleased { get; } = new SkillContainer();
        public ISkillContainer SkillsInSlot { get; } = new SkillContainer(7);

        public CharacterModel(string id, Transform transform)
        {
            ID = id;
            _transform = transform;
        }

    }

    public abstract class CharactersModel<T> : AbstractModel where T : ICharacterModel
    {
        string _currentID;

        public T Current
        {
            get
            {
                if (TryGetModel(_currentID, out T model))
                {
                    return model;
                }

                Debug.LogError($"Current model {_currentID} does not exist;");
                return default;
            }

            set => _currentID = value.ID;
        }

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
    }
}
