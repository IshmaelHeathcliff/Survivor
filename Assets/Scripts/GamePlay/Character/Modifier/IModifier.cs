using System.Collections.Generic;
using Newtonsoft.Json;


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

    public interface IModifier<T> : IModifier where T : ModifierConfig
    {
        T ModifierConfig { get; set; }
    }

    public abstract class Modifier<T> : IModifier<T> where T : ModifierConfig
    {
        protected static T GetModifierConfig(string modifierId)
        {
            return GameFrame.Interface.GetSystem<ModifierSystem>().GetModifierConfig<T>(modifierId);
        }

        public string FactoryID { get; set; }

        public abstract string GetDescription();

        public abstract void Check();
        public abstract void Register();
        public abstract void Unregister();
        public abstract void Load();

        public string ModifierID { get; set; }

        [JsonIgnore] public List<string> Keywords => ModifierConfig.Keywords;

        [JsonIgnore] public T ModifierConfig { get; set; }

        public string InstanceID { get; set; } = System.Guid.NewGuid().ToString();
    }

}
