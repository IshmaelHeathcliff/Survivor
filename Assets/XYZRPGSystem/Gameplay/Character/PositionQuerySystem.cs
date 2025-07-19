using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XYZRPGSystem.Gameplay.Character
{
    public class PositionQuerySystem : AbstractSystem
    {
        readonly Dictionary<string, List<ICharacterModel>> _models = new();

        public void RegisterModel(string tag, ICharacterModel model)
        {
            if (!_models.TryGetValue(tag, out List<ICharacterModel> models))
            {
                models = new List<ICharacterModel>();
            }

            models.Add(model);
            _models[tag] = models;
        }

        public void UnregisterModel(string tag, ICharacterModel model)
        {
            if (_models.TryGetValue(tag, out List<ICharacterModel> models))
            {
                models.Remove(model);
            }
        }

        Dictionary<string, Transform> GetTransforms(string tag)
        {
            if (_models.TryGetValue(tag, out List<ICharacterModel> models))
            {
                return models.ToDictionary(pair => pair.ID, pair => pair.Transform);
            }

            return new Dictionary<string, Transform>();
        }

        public List<Transform> Query(string tag, Vector2 position, float radius, List<string> exclude = null)
        {
            return GetTransforms(tag)
                .Where(pair => exclude == null || !exclude.Contains(pair.Key))
                .Where(pair => Vector2.Distance(pair.Value.position, position) <= radius)
                .Select(pair => pair.Value)
                .ToList();
        }

        public Transform QueryClosest(string tag, Vector2 position, float radius, List<string> exclude = null)
        {
            float radiusSquared = radius * radius;
            return GetTransforms(tag)
                .Where(pair => exclude == null || !exclude.Contains(pair.Key))
                .Where(pair => ((Vector2)pair.Value.position - position).sqrMagnitude <= radiusSquared)
                .OrderBy(pair => ((Vector2)pair.Value.position - position).sqrMagnitude)
                .Select(pair => pair.Value)
                .FirstOrDefault();
        }

        public Transform QueryClosest(string tag, Vector2 position, List<string> exclude = null)
        {
            return GetTransforms(tag)
                .Where(pair => exclude == null || !exclude.Contains(pair.Key))
                .OrderBy(pair => ((Vector2)pair.Value.position - position).sqrMagnitude)
                .Select(pair => pair.Value)
                .FirstOrDefault();
        }


        protected override void OnInit()
        {
        }
    }
}
