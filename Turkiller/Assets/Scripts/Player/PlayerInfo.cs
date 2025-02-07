using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    public string playerName = "";

    public NetworkVariable<int> score = new();



    private void Awake()
    {
        //TEST
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        int length = Random.Range(4, 10);

        string result = "";
        for (int i = 0; i < length; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        playerName = result;
        
    }

    public void AddScore(int newScore)
    {
        score.Value += newScore;
    }
}
