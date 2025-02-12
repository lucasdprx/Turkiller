using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.Collections;

public class PlayerInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new("");
    public NetworkVariable<int> skinIndex = new();

    [SerializeField] List<SkinStruct> skins;
    [SerializeField] SpriteRenderer bodySprite; 
    [SerializeField] SpriteRenderer headSprite; 
    [SerializeField] SpriteRenderer leftSprite; 
    [SerializeField] SpriteRenderer rightSprite; 
    [SerializeField] SpriteRenderer tailSprite; 

    [System.Serializable]
    public struct SkinStruct
    {
        public List<Sprite> skin;
    }


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

        if (!PlayerNameTracker.TryGetPlayerData(OwnerClientId, out string newName, out int newSkin))
            return;
        
        SetNameServerRpc(newName.Length > 1 ? newName : GetRandomName());

        skinIndex.Value = newSkin;
        SetSkin(newSkin);
        PlayerNameTracker.RemovePlayerData(OwnerClientId);
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

    public void SetSkin(int index)
    {
        bodySprite.sprite = skins[index].skin[0];
        headSprite.sprite = skins[index].skin[1];

        if (skins[index].skin.Count == 3)
        {
            leftSprite.sprite = null;
            rightSprite.sprite = null;
            tailSprite.sprite = skins[index].skin[2];
        }
        else
        {
            leftSprite.sprite = skins[index].skin[2];
            rightSprite.sprite = skins[index].skin[3];
            tailSprite.sprite = skins[index].skin[4];
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
