using Unity.Netcode;
using UnityEngine;

public class HidingPlace : NetworkBehaviour
{
    private SpriteRenderer _bush;
    private Color _clearBush;
    private Color _fullBush;
    private float _alphaValueClearBush = 0.6f;

    private void Awake()
    {
        _bush = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _clearBush = new Color(1, 1, 1, _alphaValueClearBush);
        _fullBush = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        NetworkObject netObj = collision.GetComponentInParent<NetworkObject>();
        if (!netObj)
            return;
        
        if (netObj.IsLocalPlayer)
        {
            _bush.color = _clearBush;
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
            _bush.color = _fullBush;
        }
    }
}
