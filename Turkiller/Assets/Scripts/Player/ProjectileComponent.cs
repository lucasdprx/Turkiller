using Unity.Netcode;
using UnityEngine;

public class ProjectileComponent : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 15f;
    
    private float _distanceTraveled;
    private readonly NetworkVariable<ulong> _ownerClientId = new NetworkVariable<ulong>();
    private readonly NetworkVariable<float> _damage = new NetworkVariable<float>();
    private ulong _id;
    private Transform _transform;
    private NetworkObject _networkObject;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _transform = transform;
        _networkObject = GetComponent<NetworkObject>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetVelocity();
        if (IsServer)
            _ownerClientId.Value = _id;
    }

    private void SetVelocity()
    {
        Vector2 velocity = _transform.right * _speed;
        _rigidbody.linearVelocity = velocity;
    }
    
    public void SetOwner(ulong ownerId)
    {
        _id = ownerId;
    }

    public void Init(float damage)
    {
        _damage.Value = damage;
    }

    private void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        _distanceTraveled += _speed * Time.deltaTime;
        if (!(_distanceTraveled >= _distance))
            return;
        
        DespawnServerRpc();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer || other.CompareTag("IgnoreShoot")) return;

        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.OwnerClientId == _ownerClientId.Value) return;
        
        PlayerNetworkLife playerNetworkLife = other.GetComponent<PlayerNetworkLife>();
        if (playerNetworkLife != null && NetworkManager.Singleton.LocalClientId == networkObject.OwnerClientId)
        {
            playerNetworkLife.TakeDamageServerRpc(_damage.Value, networkObject.OwnerClientId);
        }
        DespawnServerRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DespawnServerRpc()
    {
        if (_networkObject != null)
        {
            _networkObject.Despawn();
        }
    }
}