using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager instance;
    public Transform[] spawnPoints;
    public Transform[] spawnPointsSeed;
    public Transform[] spawnPointsWeathSeed;
    public Transform parentsEffectUI;

    private void Awake()
    {
        instance = this;
    }
}
