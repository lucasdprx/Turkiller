using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class SeedSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private List<Seeds> seeds;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-5, -5);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(5, 5);
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxSeeds = 10;

    private int currentSeedCount = 0;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        StartCoroutine(SpawnSeeds());

    }

    private IEnumerator SpawnSeeds()
    {
        while (true)
        {
            if (currentSeedCount < maxSeeds && seeds.Count > 0)
            {
                SpawnSeedServerRpc(OwnerClientId);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [ServerRpc]
    private void SpawnSeedServerRpc(ulong ownerClientId)
    {
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

    [ServerRpc]
    public void SeedCollectedServerRpc()
    {
        currentSeedCount--;
    }
}
