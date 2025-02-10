using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager instance;
    [SerializeField] public Transform[] respawnPoints;
    [SerializeField] public Transform[] seedSpawnPoints;


    private void Awake()
    {
        instance = this;
    }

    public Transform parentsEffectUI;
}
