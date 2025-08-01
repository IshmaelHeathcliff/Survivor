using System.Collections.Generic;
using System.Linq;
using XYZRPGSystem.Gameplay.Skill;
using Sirenix.OdinInspector;

namespace XYZRPGSystem.Data.Config
{
    public class SkillConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string IconAddress { get; set; } = "Assets/Textures/UI/icons.aseprite[icons_74]";
        [ShowInInspector] public List<string> Keywords { get; set; }

        [ShowInInspector] public SkillRarity Rarity { get; set; }
        [ShowInInspector] public string Description { get; set; }

        [ShowInInspector] public bool ReleaseOnAcquire { get; set; }


        [ShowInInspector]
        [ListDrawerSettings(ShowIndexLabels = true)]
        [TypeFilter("GetFilteredTypeList")]
        public List<SkillEffectConfig> SkillEffectConfigsOnEnable { get; set; }

        [ShowInInspector] public virtual List<SkillEffectConfig> SkillEffectConfigsOnUse { get; set; }

        IEnumerable<System.Type> GetFilteredTypeList()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(SkillEffectConfig).IsAssignableFrom(type) &&
                               !type.IsAbstract &&
                               type != typeof(AttackEffectConfig));
        }
    }

    public class RepetitiveSkillConfig : SkillConfig
    {
        [ShowInInspector] public float Cooldown { get; set; } = 1f;
        [ShowInInspector] public bool IsAutoUse { get; set; } = true;
    }

    public class OneTimeSkillConfig : SkillConfig
    {

    }


    public class AttackSkillConfig : RepetitiveSkillConfig
    {
        [ShowInInspector] public List<AttackEffectConfig> AttackEffectConfigs { get; set; }
        [ShowInInspector] public float Damage { get; set; } = 0f;
        [ShowInInspector] public float CriticalChance { get; set; } = 0f;
        [ShowInInspector] public float CriticalMultiplier { get; set; } = 150f;
        [ShowInInspector] public float AttackArea { get; set; } = 1f;
        [ShowInInspector] public float AttackRange { get; set; } = 4f;
        [ShowInInspector] public float AttackSpeed { get; set; } = 1f;
        [ShowInInspector] public float Duration { get; set; } = 3f;

        [ShowInInspector] public float WoodOnUse { get; set; } = 0f;
    }

    public class ProjectileAttackSkillConfig : AttackSkillConfig
    {
        [ShowInInspector] public float ProjectileCount { get; set; } = 1f;
        [ShowInInspector] public float ProjectileSpeed { get; set; } = 10f;
        [ShowInInspector] public float ChainCount { get; set; } = 0f;
        [ShowInInspector] public float PenetrateCount { get; set; } = 0f;
        [ShowInInspector] public float SplitCount { get; set; } = 0f;
        [ShowInInspector] public bool IsTargetLocked { get; set; } = false;
        [ShowInInspector] public bool CanReturn { get; set; } = false;
    }

    public class SelfAttackSkillConfig : AttackSkillConfig
    {
        [ShowInInspector] public bool CanReturn { get; set; } = false;
        [ShowInInspector] public bool IsTargetLocked { get; set; } = false;

    }
}
