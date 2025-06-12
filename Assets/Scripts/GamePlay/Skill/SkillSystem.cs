using System.Collections.Generic;
using Character;
using Character.Modifier;
using SaveLoad;
using UnityEngine;

namespace Skill
{
    public class SkillCreateEnv
    {
        public ICharacterModel Model;
        public ModifierSystem ModifierSystem;
        public ResourceSystem ResourceSystem;
        public CountSystem CountSystem;
        public SkillSystem SkillSystem;
    }

    public class SkillSystem : AbstractSystem
    {
        readonly Dictionary<string, SkillConfig> _skillConfigCache = new();
        const string JsonPath = "Preset";
        const string JsonName = "Skills.json";

        public SkillCreateEnv SkillCreateEnv { get; set; }

        void Load()
        {
            _skillConfigCache.Clear();
            List<SkillConfig> skillConfigList = this.GetUtility<SaveLoadUtility>().Load<List<SkillConfig>>(JsonName, JsonPath);
            foreach (SkillConfig skillConfig in skillConfigList)
            {
                _skillConfigCache.Add(skillConfig.ID, skillConfig);
            }
        }

        public SkillConfig GetSkillConfig(string id)
        {
            if (_skillConfigCache.TryGetValue(id, out SkillConfig skillConfig))
            {
                return skillConfig;
            }

            Debug.LogError($"SkillConfig not found: {id}");
            return null;
        }

        public void SetEnv(ICharacterModel model)
        {
            if (model != null)
            {
                SkillCreateEnv.Model = model;
            }

            if (!CheckEnv())
            {
                Debug.LogError("SkillCreateEnv is not set correctly");
            }
        }

        public ISkill CreateSkill(string id, ICharacterModel model = null)
        {
            SetEnv(model);

            return SkillConfigLoader.CreateSkill(GetSkillConfig(id), SkillCreateEnv);
        }

        public void AcquireSkill(string id, ICharacterModel model = null)
        {
            ISkill skill = CreateSkill(id, model);

            ISkillContainer SkillsInSlot = SkillCreateEnv.Model.SkillsInSlot;

            if (SkillsInSlot.Count >= SkillsInSlot.MaxCount)
            {
                Debug.Log($"技能槽位已满，最大数量: {SkillsInSlot.MaxCount}");
                return;
            }

            if (!SkillsInSlot.AddSkill(skill))
            {
                return;
            }

            this.SendEvent(new SkillAcquiredEvent(skill, SkillCreateEnv.Model));
        }

        public void ReleaseSkill(string id, ICharacterModel model = null)
        {
            SetEnv(model);

            if (!SkillCreateEnv.Model.SkillsInSlot.ReleaseSkill(id, out ISkill skill))
            {
                return;
            }

            if (!SkillCreateEnv.Model.SkillsReleased.AddSkill(skill))
            {
                return;
            }

            this.SendEvent(new SkillReleasedEvent(skill, SkillCreateEnv.Model));
        }

        public void RemoveSkill(string id, ICharacterModel model = null)
        {
            SetEnv(model);

            if (SkillCreateEnv.Model.SkillsInSlot.RemoveSkill(id) || SkillCreateEnv.Model.SkillsReleased.RemoveSkill(id))
            {
                this.SendEvent(new SkillRemovedEvent(id, SkillCreateEnv.Model));
            }
        }

        public void ClearSkill(ICharacterModel model)
        {
            foreach (ISkill skill in model.GetAllSkills())
            {
                this.SendEvent(new SkillRemovedEvent(skill.ID, model));
            }

            model.SkillsInSlot.Clear();
            model.SkillsReleased.Clear();
        }

        public void SetSkillSlotCount(int count, ICharacterModel model = null)
        {
            SetEnv(model);
            SkillCreateEnv.Model.SkillSlotCount = count;
            this.SendEvent(new SkillSlotCountChangedEvent(count, SkillCreateEnv.Model));
        }

        public bool CheckEnv()
        {
            if (SkillCreateEnv.Model == null)
            {
                Debug.LogError("Model is null");
                return false;
            }

            return true;
        }

        protected override void OnInit()
        {
            Load();
            SkillCreateEnv = new()
            {
                ModifierSystem = this.GetSystem<ModifierSystem>(),
                ResourceSystem = this.GetSystem<ResourceSystem>(),
                CountSystem = this.GetSystem<CountSystem>(),
                SkillSystem = this
            };
        }
    }
}
