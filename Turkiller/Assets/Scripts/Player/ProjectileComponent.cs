using Unity.Netcode;
using UnityEngine;

public class ProjectileComponent : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 10f;
    private float _distanceTraveled;

    private void Update()
    {
        if (!IsServer) return;

        transform.position += transform.right * (_speed * Time.deltaTime);
        _distanceTraveled += _speed * Time.deltaTime;

        if (_distanceTraveled >= _distance)
        {
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        PlayerNetworkLife playerNetworkLife = collision.gameObject.GetComponent<PlayerNetworkLife>();
        if (playerNetworkLife != null)
        {
            playerNetworkLife.TakeDamageServerRpc(10); // Apply 10 points of damage
            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(true);
            }
        }
    }
}