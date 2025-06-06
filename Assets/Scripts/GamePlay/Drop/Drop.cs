using Character.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            this.GetModel<PlayersModel>().Current.Coin.Value += 1;
            Addressables.ReleaseInstance(gameObject);
        }
    }
}
