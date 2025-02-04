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
    private Transform _transform;
    private NetworkObject _networkObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _transform = transform;
        _networkObject = GetComponent<NetworkObject>();
    }

    public void SetOwner(ulong ownerId)
    {
        _ownerClientId = ownerId;
    }

    private void Update()
    {
        if (!IsServer) return;

        MoveProjectile();
    }

    private void MoveProjectile()
    {
        Vector2 movement = _transform.right * (_speed * Time.deltaTime);
        //_rigidbody.MovePosition(_rigidbody.position + movement);
        _transform.position += _transform.right * (_speed * Time.deltaTime);;
        _distanceTraveled += _speed * Time.deltaTime;

        if (!(_distanceTraveled >= _distance))
            return;
        
        if (_networkObject != null)
            _networkObject.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        NetworkObject networkObject = other.GetComponent<NetworkObject>();
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