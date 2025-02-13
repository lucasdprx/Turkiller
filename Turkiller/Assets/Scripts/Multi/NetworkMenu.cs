using System;
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

    public void StartClient(GameObject networkMenu, string playerName, int skinIndex, int voiceIndex)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
        byte[] skinBytes = BitConverter.GetBytes(skinIndex);
        byte[] voiceBytes = BitConverter.GetBytes(voiceIndex);

        byte[] connectionData = new byte[nameBytes.Length + 1 + skinBytes.Length + voiceBytes.Length];

        connectionData[0] = (byte)nameBytes.Length;
        nameBytes.CopyTo(connectionData, 1);
        skinBytes.CopyTo(connectionData, 1 + nameBytes.Length);
        voiceBytes.CopyTo(connectionData, 1 + nameBytes.Length + skinBytes.Length);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = connectionData;


        bool success = NetworkManager.Singleton.StartClient();

        if (!success)
        {
            Debug.Log("Failed to start client");
        }
        else
        {
            networkMenu.SetActive(false);
        }
    }
    public void StartHost(GameObject networkMenu, string playerName, int skinIndex, int voiceIndex)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
        byte[] skinBytes = BitConverter.GetBytes(skinIndex);
        byte[] voiceBytes = BitConverter.GetBytes(voiceIndex);

        byte[] connectionData = new byte[nameBytes.Length + 1 + skinBytes.Length + voiceBytes.Length];

        connectionData[0] = (byte)nameBytes.Length;
        nameBytes.CopyTo(connectionData, 1);
        skinBytes.CopyTo(connectionData, 1 + nameBytes.Length);
        voiceBytes.CopyTo(connectionData, 1 + nameBytes.Length + skinBytes.Length);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = connectionData;


        if (!NetworkManager.Singleton.StartHost()) return;
            
        networkMenu.SetActive(false);
    }
}
