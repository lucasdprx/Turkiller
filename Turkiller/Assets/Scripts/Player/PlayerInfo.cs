using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new("");

    public NetworkVariable<int> score = new();

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        if (PlayerNameTracker.TryGetPlayerName(OwnerClientId, out string newName))
        {
            print(newName);
            if (newName.Length > 1)
            {
                playerName.Value = newName;
            }
            else
            {
                FixedString64Bytes n = GetRandomName();
                playerName.Value = n;
            }

            PlayerNameTracker.RemovePlayerName(OwnerClientId);
        }

        
    }

    private void Awake()
    {
        
        
        
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
