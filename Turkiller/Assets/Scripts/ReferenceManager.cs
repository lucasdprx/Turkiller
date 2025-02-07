using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform parentsEffectUI;

    public string playerName;
}
