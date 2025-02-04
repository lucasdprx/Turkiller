using Unity.Netcode;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    private void Start()
    {
        if (!Application.isBatchMode)
            return;
        
        NetworkManager.Singleton.StartServer();
        Debug.Log("Start server !");
    }

    public void StartClient(GameObject buttonClient)
    {
        bool success = NetworkManager.Singleton.StartClient();

        if (!success)
        {
            Debug.Log("Failed to start client");
        }
        else
        {
            buttonClient.SetActive(false);
        }
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
