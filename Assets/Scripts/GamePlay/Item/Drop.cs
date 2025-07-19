using UnityEngine;
using UnityEngine.AddressableAssets;
using XYZRPGSystem.Gameplay.Character;
using XYZRPGSystem.Gameplay.Damage;
using XYZRPGSystem.Gameplay.Item;

namespace Gameplay.Item
{
    public class Drop : MonoBehaviour, IController
    {
        public string DropID;

        public IArchitecture GetArchitecture()
        {
            return GameFrame.Interface;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.GetSystem<ResourceSystem>().AcquireResource("Coin", 1, other.GetComponent<IDamageable>().CharacterController.CharacterModel as IHasResources);
                Addressables.ReleaseInstance(gameObject);
            }
        }
    }
}
