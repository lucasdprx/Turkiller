using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SeedSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private List<Seeds> seeds;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10, -10);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(10, 10);
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxSeeds = 10;

    private int currentSeedCount = 0;

    public override void OnNetworkSpawn()
    {
        print("1");
        if(!IsServer) return;

        print("2");
        StartCoroutine(SpawnSeeds());
    }

    private IEnumerator SpawnSeeds()
    {
        while (true)
        {
            if (currentSeedCount < maxSeeds && seeds.Count > 0)
            {
                print("3");
                SpawnSeedServerRpc(OwnerClientId);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnSeedServerRpc(ulong ownerClientId)
    {
        print("4");
        if (seeds.Count == 0) return;

        Seeds randomSeed = seeds[Random.Range(0, seeds.Count)];

        Vector2 randomPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject newSeed = Instantiate(seedPrefab, randomPosition, Quaternion.identity);
        newSeed.GetComponent<SeedObject>().Init(randomSeed, this);
        currentSeedCount++;

        NetworkObject networkObject = newSeed.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            newSeed.GetComponent<SeedObject>().SetOwner(ownerClientId); // Assignation de l'ID
            networkObject.Spawn(true);
        }
    }

    [Rpc(SendTo.Server)]
    public void SeedCollectedServerRpc()
    {
        currentSeedCount--;
    }
}
