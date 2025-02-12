using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new("");

    public NetworkVariable<int> score = new();
    public TextMeshProUGUI usernameText;
    public CircleCollider2D circleCollider;
    public GameObject _bodyPlayer;

    private void Awake()
    {
        playerName.OnValueChanged += OnNameChanged;
    }
    private void OnNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        usernameText.text = newValue.ToString();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        if (!PlayerNameTracker.TryGetPlayerName(OwnerClientId, out string newName))
            return;
        
        SetNameServerRpc(newName.Length > 1 ? newName : GetRandomName());
        PlayerNameTracker.RemovePlayerName(OwnerClientId);
    }
    
    [Rpc(SendTo.Server)]
    private void SetNameServerRpc(string newName)
    {
        playerName.Value = newName;
        usernameText.text = newName;
        SetNameClientRpc(newName);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetNameClientRpc(string newName)
    {
        usernameText.text = newName;
        IReadOnlyDictionary<ulong, NetworkClient> players = NetworkManager.Singleton.ConnectedClients;
        foreach (KeyValuePair<ulong, NetworkClient> player in players)
        {
            if (player.Value.PlayerObject == null)
                continue;
            
            PlayerInfo playerInfo = player.Value.PlayerObject.GetComponent<PlayerInfo>();
            playerInfo.usernameText.text = playerInfo.playerName.Value.ToString();
        }
    }

    private string GetRandomName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        int length = Random.Range(4, 10);

        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        return result;
    }

    public void AddScore(int newScore)
    {
        score.Value += newScore;
    }
}
