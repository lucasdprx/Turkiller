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

        if (request.Payload.Length > 0)
        {
            playerName = Encoding.UTF8.GetString(request.Payload);
        }

        Debug.Log($"Joueur {clientId} se connecte avec le nom : {playerName}");

        response.Approved = true;
        response.CreatePlayerObject = true;

        PlayerNameTracker.AddPlayerName(clientId, playerName);
    }
}
