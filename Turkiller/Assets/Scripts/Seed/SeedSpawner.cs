using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SeedSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private List<Seeds> seeds;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxSeeds = 10;

    private int currentSeedCount = 0;
    private HashSet<Transform> occupiedSpawnPoints = new HashSet<Transform>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

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

    [Rpc(SendTo.Server)]
    private void SpawnSeedServerRpc(ulong ownerClientId)
    {
        if (seeds.Count == 0) return;

        Seeds randomSeed = seeds[Random.Range(0, seeds.Count)];
        Transform spawnPoint = GetRandomAvailableSpawnPoint();

        if (spawnPoint == null) return;

        GameObject newSeed = Instantiate(seedPrefab, spawnPoint.position, Quaternion.identity);
        newSeed.GetComponent<SeedObject>().Init(randomSeed, this);
        occupiedSpawnPoints.Add(spawnPoint);
        currentSeedCount++;

        NetworkObject networkObject = newSeed.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            newSeed.GetComponent<SeedObject>().SetOwner(ownerClientId);
            networkObject.Spawn(true);
        }
    }

    [Rpc(SendTo.Server)]
    public void SeedCollectedServerRpc(Vector3 spawnPosition)
    {
        currentSeedCount--;

        foreach (Transform point in ReferenceManager.instance.respawnPoints)
        {
            if (point.position == spawnPosition)
            {
                occupiedSpawnPoints.Remove(point);
                break;
            }
        }
    }


    private Transform GetRandomAvailableSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in ReferenceManager.instance.seedSpawnPoints)
        {
            if (!occupiedSpawnPoints.Contains(point))
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
        {
            Debug.LogWarning("No available spawn points!");
            return null;
        }

        return availablePoints[Random.Range(0, availablePoints.Count)];
    }
}
