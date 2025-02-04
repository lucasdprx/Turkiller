using Unity.Netcode;
using UnityEngine;

public class ProjectileComponent : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 10f;
    [SerializeField] private string tagPlayer = "Player";
    
    private float _distanceTraveled;
    private ulong _ownerClientId;
    private Transform _transform;
    private NetworkObject _networkObject;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _transform = transform;
        _networkObject = GetComponent<NetworkObject>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity()
    {
        Vector2 velocity = _transform.right * _speed;
        _rigidbody.linearVelocity = velocity;
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

        PlayerNetworkLife playerNetworkLife = other.GetComponent<PlayerNetworkLife>();
        if (playerNetworkLife == null)
            return;
        
        playerNetworkLife.TakeDamageServerRpc(10);
        if (_networkObject != null)
        {
            _networkObject.Despawn();
        }
    }
}