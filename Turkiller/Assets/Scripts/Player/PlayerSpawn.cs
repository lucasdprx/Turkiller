using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform GetRandomSpawnPoint()
    {
        if (ReferenceManager.instance.respawnPoints.Length == 0)
        {
            Debug.LogWarning("No respawn points assigned!");
            return null;
        }

        return ReferenceManager.instance.respawnPoints[Random.Range(0, ReferenceManager.instance.respawnPoints.Length)];
    }
}
