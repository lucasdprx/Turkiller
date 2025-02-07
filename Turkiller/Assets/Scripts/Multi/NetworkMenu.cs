using System.Text;
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

    public void StartClient(GameObject buttonClient, string playerName)
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(playerName);


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
    public void StartHost(GameObject buttonClient, string playerName)
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(playerName);


        if (!NetworkManager.Singleton.StartHost()) return;
            
        buttonClient.SetActive(false);
    }
}
