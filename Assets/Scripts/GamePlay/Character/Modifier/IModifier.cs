using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;


namespace Character.Modifier
{
    public interface IModifier
    {
        void Check();
        void Register();
        void Unregister();
        string ModifierID { get; set; }
        string FactoryID { get; set; }
        string GetDescription();
        List<string> Keywords { get; }
        string InstanceID { get; set; }
        void Load();
    }

    public interface IModifier<T> : IModifier where T : ModifierInfo
    {
        T ModifierInfo { get; set; }
    }

    public abstract class Modifier<T> : IModifier<T> where T : ModifierInfo
    {
        protected static T GetModifierInfo(string modifierId)
        {
            return GameFrame.Interface.GetSystem<ModifierSystem>().GetModifierInfo<T>(modifierId);
        }

        public string FactoryID { get; set; }

        public abstract string GetDescription();

        public abstract void Check();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();

        public string ModifierID { get; set; }

        [JsonIgnore] public List<string> Keywords => ModifierInfo.Keywords;

        [JsonIgnore] public T ModifierInfo { get; set; }

        public string InstanceID { get; set; } = System.Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Modifier的生成信息
    /// </summary>
    [Serializable]
    public abstract class ModifierInfo
    {
        [ShowInInspector] public string ModifierID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string PositiveDescription { get; set; }
        [ShowInInspector] public string NegativeDescription { get; set; }
        [ShowInInspector] public List<string> Keywords { get; set; }
    }
}
