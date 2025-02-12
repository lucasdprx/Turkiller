using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new("");
    public NetworkVariable<int> skinIndex = new();

    [SerializeField] List<Sprite> skins;
    [SerializeField] SpriteRenderer playerSprite; 

    public NetworkVariable<int> score = new();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (PlayerNameTracker.TryGetPlayerData(OwnerClientId, out string newName, out int newSkin))
            {
                if (newName.Length > 1)
                {
                    playerName.Value = newName;
                }
                else
                {
                    FixedString64Bytes n = GetRandomName();
                    playerName.Value = n;
                }

                skinIndex.Value = newSkin;

                SetSkin(newSkin);

                PlayerNameTracker.RemovePlayerData(OwnerClientId);
            }
        }

        playerSprite.sprite = skins[skinIndex.Value];
    }

    public void SetSkin(int index)
    {
        playerSprite.sprite = skins[index];
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
