using Unity.Netcode;
using UnityEngine;

public class ProjectileComponent : NetworkBehaviour
{
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _distance = 15f;
    [SerializeField] private GameObject _eggParticlesPrefab;

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

        if(Input.GetKeyDown(KeyCode.O))
        {
            DespawnServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void MoveProjectile()
    {
        _distanceTraveled += _speed * Time.deltaTime;
        if (!(_distanceTraveled >= _distance))
            return;
        
        DespawnServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer || other.CompareTag("IgnoreShoot")) return;

        NetworkObject networkObject = other.GetComponentInParent<NetworkObject>();
        if (networkObject != null && networkObject.OwnerClientId == _ownerClientId.Value) return;
        
        PlayerNetworkLife playerNetworkLife = other.GetComponentInParent<PlayerNetworkLife>();
        if (playerNetworkLife != null && NetworkManager.Singleton.LocalClientId == networkObject.OwnerClientId)
        {
            playerNetworkLife.TakeDamageServerRpc(_damage.Value, networkObject.OwnerClientId, _ownerClientId.Value);
        }
        
        PlayLocalParticles();
        DespawnServerRpc(NetworkManager.Singleton.LocalClientId);
        AudioManager.Instance.PlaySFX("impact egg", false, this.transform.position);
    }


    private void PlayLocalParticles()
    {
        GameObject particles = Instantiate(_eggParticlesPrefab, _transform.position, Quaternion.identity);
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
        Destroy(particles, ps.main.duration);
    }



    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DespawnServerRpc(ulong senderClientId)
    {
        if (!IsServer) return;

        SpawnParticlesClientRpc(_transform.position, senderClientId);

        _networkObject.Despawn();
    }



    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnParticlesClientRpc(Vector3 position, ulong ignoreClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == ignoreClientId) return; // evite le double effet

        GameObject particles = Instantiate(_eggParticlesPrefab, position, Quaternion.identity);
        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
        Destroy(particles, ps.main.duration);
    }



}