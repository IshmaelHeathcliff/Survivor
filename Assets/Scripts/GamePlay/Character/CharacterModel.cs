using System.Collections.Generic;
using System.Linq;
using GamePlay.Character.Stat;
using GamePlay.Character.State;
using GamePlay.Skill;
using UnityEngine;


namespace GamePlay.Character
{
    public interface IHasSkill
    {
        SkillPool SkillPool { get; set; }
        ISkillContainer SkillsReleased { get; }
        ISkillContainer SkillsInSlot { get; }
        int SkillSlotCount { get; set; }
        ISkill GetSkill(string id);
        bool TryGetSkill(string id, out ISkill skill);
        IEnumerable<ISkill> GetAllSkills();
        bool HasSkills(IEnumerable<string> ids);
    }

    public interface IHasState
    {
        IStateContainer StateContainer { get; }
    }

    public interface ICanCountValue
    {
        Dictionary<string, ValueCounter> CountValues { get; }
    }

    public interface ICharacterModel : IHasSkill, IHasState, ICanCountValue
    {
        ICharacterController Controller { get; set; }

        string ID { get; set; }
        float Speed { get; set; }
        Vector2 Position { get; set; }
        Vector2 Direction { get; set; }
        CharacterStats Stats { get; }

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
        public CharacterStats Stats { get; } = new CharacterStats();
        public IStateContainer StateContainer { get; } = new StateContainer();
        public SkillPool SkillPool { get; set; } = new();


        public int SkillSlotCount
        {
            get => SkillsInSlot.MaxCount;
            set => SkillsInSlot.MaxCount = value;
        }

        public ISkillContainer SkillsReleased { get; } = new SkillContainer();
        public ISkillContainer SkillsInSlot { get; } = new SkillContainer(7);

        public ICharacterController Controller { get; set; }

        public Dictionary<string, ValueCounter> CountValues { get; } = new();

        public void BindTransform(Transform transform)
        {
            _transform = transform;
        }

        public ISkill GetSkill(string id)
        {
            if (SkillsInSlot.TryGetSkill(id, out ISkill skill))
            {
                return skill;
            }

            if (SkillsReleased.TryGetSkill(id, out skill))
            {
                return skill;
            }

            return null;
        }

        public bool TryGetSkill(string id, out ISkill skill)
        {
            return SkillsInSlot.TryGetSkill(id, out skill) || SkillsReleased.TryGetSkill(id, out skill);
        }

        public IEnumerable<ISkill> GetAllSkills()
        {
            return SkillsInSlot.GetAllSkills().Concat(SkillsReleased.GetAllSkills());
        }

        public bool HasSkills(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                if (!(SkillsInSlot.HasSkill(id) || SkillsReleased.HasSkill(id)))
                {
                    return false;
                }
            }

            return true;
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
            if (TryGetModel(id, out TModel model))
            {
                model.SkillsInSlot.Clear();
                model.SkillsReleased.Clear();
                model.StateContainer.Clear();
                model.Controller = null;
                Models.Remove(id);
            }
        }
    }
}
