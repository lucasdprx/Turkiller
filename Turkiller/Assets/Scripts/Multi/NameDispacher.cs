using System;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NameDispacher : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        ulong clientId = request.ClientNetworkId;
        string playerName = "Joueur";
        int skinIndex = 0;
        int voiceIndex = 0;

        if (request.Payload.Length < 1)
        {
            Debug.LogError($"Client {clientId} a envoyé un payload vide !");
            response.Approved = false;
            return;
        }

        if (request.Payload.Length > 0)
        {

            byte nameLength = request.Payload[0];

            if (request.Payload.Length < 1 + nameLength)
            {
                Debug.LogError($"Client {clientId} a envoyé un nom trop long ({nameLength} chars) pour un payload de {request.Payload.Length} bytes !");
                response.Approved = false;
                return;
            }

            playerName = Encoding.UTF8.GetString(request.Payload, 1, nameLength);
            skinIndex = BitConverter.ToInt32(request.Payload, 1 + nameLength);
            voiceIndex = BitConverter.ToInt32(request.Payload, 1 + nameLength + 4);

        }

        Debug.Log($"Joueur {clientId} se connecte avec le nom : {playerName}, l'index de skin : {skinIndex} et l'index de voix : {voiceIndex}");

        response.Approved = true;
        response.CreatePlayerObject = true;

        PlayerNameTracker.AddPlayerData(clientId, playerName, skinIndex, voiceIndex);
    }
}
