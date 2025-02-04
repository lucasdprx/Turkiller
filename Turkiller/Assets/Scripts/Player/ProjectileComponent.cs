using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileComponent : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 10f;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private string tagPlayer;
    private float _distanceTraveled;
    private ulong _ownerClientId;
    
    public void SetOwner(ulong ownerId)
    {
        _ownerClientId = ownerId;
    }

    private void Update()
    {
        if (!IsServer) return;

        Vector2 movement = transform.right * (_speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.OwnerClientId == _ownerClientId)
        {
            return;
        }

        Debug.Log("Projectile hit something");
        // PlayerNetworkLife playerNetworkLife = other.gameObject.GetComponent<PlayerNetworkLife>();
        // if (playerNetworkLife != null)
        // {
        //     playerNetworkLife.TakeDamageServerRpc(10);
        //     NetworkObject projNetworkObject = gameObject.GetComponent<NetworkObject>();
        //     if (projNetworkObject != null)
        //     {
        //         projNetworkObject.Despawn(true);
        //     }
        // }
    }

}