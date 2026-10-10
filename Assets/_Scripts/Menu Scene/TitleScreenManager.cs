using Unity.Netcode;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public void StartNetworkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.CreateNewGame();
        StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());
    }

}
