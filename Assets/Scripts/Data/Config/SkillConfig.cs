using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace Data.Config
{
    public class SkillConfig
    {
        [ShowInInspector] public string ID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public List<string> Keywords { get; set; }
        [ShowInInspector] public string Description { get; set; }

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
        [ShowInInspector] public float Cooldown { get; set; }
    }

    public class OneTimeSkillConfig : SkillConfig
    {

    }


    public class AttackSkillConfig : RepetitiveSkillConfig
    {
        [ShowInInspector] public List<AttackEffectConfig> AttackEffectConfigs { get; set; }
        [ShowInInspector] public float Damage { get; set; }
        [ShowInInspector] public float CriticalChance { get; set; }
        [ShowInInspector] public float CriticalMultiplier { get; set; }
        [ShowInInspector] public float AttackArea { get; set; }
        [ShowInInspector] public float Duration { get; set; }
        [ShowInInspector] public float ProjectileCount { get; set; }
        [ShowInInspector] public float ProjectileSpeed { get; set; }
        [ShowInInspector] public bool ReleaseOnAcquire { get; set; }
    }
}
