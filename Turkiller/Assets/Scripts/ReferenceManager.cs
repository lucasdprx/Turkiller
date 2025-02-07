using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager instance;
    [SerializeField] public Transform[] respawnPoints;


    private void Awake()
    {
        instance = this;
    }

    public Transform parentsEffectUI;
}
