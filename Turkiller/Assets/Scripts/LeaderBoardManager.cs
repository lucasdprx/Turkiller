using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class LeaderBoardManager : NetworkBehaviour
{
    public static LeaderBoardManager Instance;

    public GameObject elementPrefab;

    public Transform elementsParent;

    List<LeaderBoardElement> elements = new();

    List<PlayerInfo> playersStocked = new();

    public int maxElementsShown;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < maxElementsShown; i++)
        {
            GameObject go = Instantiate(elementPrefab, elementsParent);

            elements.Add(go.GetComponent<LeaderBoardElement>());

            go.SetActive(false);
        }

        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += RefreshPlayerList;

        RefreshPlayerList();

        UpdateLeaderboard(0, 0);
    }

    private void RefreshPlayerList()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            var player = client.Value.PlayerObject.GetComponent<PlayerInfo>();
            if (player != null && !playersStocked.Contains(player))
            {
                playersStocked.Add(player);
                player.score.OnValueChanged += UpdateLeaderboard;
            }
        }
    }

    private void RefreshPlayerList(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(obj, out var client))
        {
            var player = client.PlayerObject.GetComponent<PlayerInfo>();
            if (player != null && !playersStocked.Contains(player))
            {
                playersStocked.Add(player);
                player.score.OnValueChanged += UpdateLeaderboard;
            }

            UpdateLeaderboard(0, 0);
        }
    }

    public void UpdateLeaderboard(int previousVal, int newVal)
    {
        if (!IsServer) return;

        List<(FixedString64Bytes clientName, int score)> playerScores = new();

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            var player = client.Value.PlayerObject.GetComponent<PlayerInfo>();
            if (player != null)
            {
                playerScores.Add((player.playerName, player.score.Value));
            }
        }

        playerScores = playerScores.OrderByDescending(p => p.score).ToList();

        UpdateLeaderboardVisualsClientRpc(playerScores.Select(p => p.clientName).ToArray(),
                                   playerScores.Select(p => p.score).ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UpdateLeaderboardVisualsClientRpc(FixedString64Bytes[] clientNames, int[] scores)
    {
        int size = Mathf.Min(clientNames.Length, maxElementsShown);
        int diff = maxElementsShown - size;



        for (int i = 0; i < size; i++)
        {
            elements[i].gameObject.SetActive(true);
            elements[i].UpdateTexts(i + 1, clientNames[i], scores[i]);

        }

        for (int i = 0;i < diff; i++)
        {
            elements[size+i].gameObject.SetActive(false);
        }
    }
}
