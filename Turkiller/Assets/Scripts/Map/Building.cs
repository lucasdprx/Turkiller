using Unity.Netcode;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _building;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        NetworkObject netObj = collision.GetComponentInParent<NetworkObject>();
        if (!netObj)
            return;
        
        if (netObj.IsLocalPlayer)
        {
            _building.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        NetworkObject netObj = collision.GetComponentInParent<NetworkObject>();
        if (!netObj)
            return;
        
        if (netObj.IsLocalPlayer)
        {
            _building.SetActive(true);
        }
    }
}
