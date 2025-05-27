using Character.Player;
using UnityEngine;

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
            this.GetModel<PlayersModel>().Default().Coin.Value += 1;
            Destroy(gameObject);
        }
    }
}