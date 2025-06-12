using GamePlay.Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GamePlay.Item
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
                this.GetModel<PlayersModel>().Current.Resources["Coin"].Value += 1;
                Addressables.ReleaseInstance(gameObject);
            }
        }
    }
}
