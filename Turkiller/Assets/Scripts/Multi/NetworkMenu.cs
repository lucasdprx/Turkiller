using Unity.Netcode;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    private void Start()
    {
        if (!Application.isBatchMode)
            return;
        
        NetworkManager.Singleton.StartServer();
        Debug.Log("Start server");
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
