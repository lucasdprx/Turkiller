using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform GetRandomSpawnPoint()
    {
        if (ReferenceManager.instance.spawnPoints.Length == 0)
        {
            Debug.LogWarning("No respawn points assigned!");
            return null;
        }

        return ReferenceManager.instance.spawnPoints[Random.Range(0, ReferenceManager.instance.spawnPoints.Length)];
    }
}
