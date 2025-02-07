using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public string playerName = "";

    public NetworkVariable<int> score = new();



    private void Awake()
    {
        string newName = ReferenceManager.instance.playerName;
        if(newName.Length > 1)
        {
            playerName = newName;
        }
        else
        {
            playerName = GetRandomName();
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
