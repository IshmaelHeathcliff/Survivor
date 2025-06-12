using Sirenix.OdinInspector;

namespace GamePlay.Drop
{
    public class DropConfig
    {
        [ShowInInspector] public string DropID { get; set; }
        [ShowInInspector] public string Name { get; set; }
        [ShowInInspector] public string DropPrefab { get; set; }
    }
}
