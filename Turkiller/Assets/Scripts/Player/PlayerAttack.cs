using System.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _baseDamage = 10;
    [SerializeField] private PlayerEffects _effects;
    
    [SerializeField] private float _distanceAttackSpeed = 0.6f;
    [SerializeField] private float _recoilDistanceAttack = 4;
    
    [SerializeField] private float _recoilMeleeAttack = 4;
    [SerializeField] private float _MeleeAttackSpeed = 0.6f;
    [SerializeField] private Transform _meleeAttackPoint;

    private Rigidbody2D _rb;
    private float _attackTimer;
    private bool _isAttacking;
    private bool _isDistanceAttack = true;
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _transform = transform;
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;
        if (!IsOwner || !Input.GetMouseButton(0)) return;

        if (_isDistanceAttack && _attackTimer >= _distanceAttackSpeed)
        {
            _attackTimer = 0;
            
            Vector3 dir = GetMousePosition(_camera) - _spawnPoint.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion direction = Quaternion.AngleAxis(angle, new Vector3(0, _spawnPoint.rotation.y, 1));

            ulong ownerClientId = GetComponent<NetworkObject>().OwnerClientId;
            _rb.AddForce(-dir.normalized * _recoilDistanceAttack, ForceMode2D.Impulse);
            RequestDistanceAttackServerRpc(ownerClientId, _spawnPoint.position, direction);
        }
        
        else if (!_isDistanceAttack && _attackTimer >= _MeleeAttackSpeed)
        {
            _attackTimer = 0;
            Vector3 dir = GetMousePosition(_camera) - _spawnPoint.position;
            _rb.AddForce(dir.normalized * _recoilMeleeAttack, ForceMode2D.Impulse);
            StartCoroutine(TimeMeleeAttack());
        }
    }
    private IEnumerator TimeMeleeAttack()
    {
        _playerController.FreezeInput(true);
        yield return new WaitForSeconds(0.2f);
        Collider2D[] results =  Physics2D.OverlapBoxAll(_meleeAttackPoint.position, Vector2.one * 2, 0);
        foreach (Collider2D result in results)
        {
            if (result.GetComponent<PlayerNetworkLife>() == null)
                continue;
            
            NetworkObject networkObjectResult = result.GetComponent<NetworkObject>();
            NetworkObject networkObjectPlayer = GetComponent<NetworkObject>();

            if (networkObjectResult == null || networkObjectPlayer == null ||
                networkObjectResult.OwnerClientId == networkObjectPlayer.OwnerClientId) continue;
            
            result.GetComponent<PlayerNetworkLife>().TakeDamageServerRpc(20, networkObjectResult.OwnerClientId);
        }
        yield return new WaitForSeconds(0.2f);
        _playerController.FreezeInput(false);
    }
    
    public static Vector3 GetMousePosition(Camera cam)
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    [ServerRpc]
    private void RequestDistanceAttackServerRpc(ulong ownerClientId, Vector3 spawnPoint, Quaternion direction)
    {
        GameObject projectile = Instantiate(_projectilePrefab, spawnPoint, direction);

        if (!projectile.TryGetComponent(out ProjectileComponent projectileComponent))
            return;
        
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        if (networkObject == null)
            return;
        
        projectileComponent.SetOwner(ownerClientId);
        projectileComponent.Init(_baseDamage * _effects.GetEffect(Bonus.DamageMultiplier).max);
        networkObject.Spawn(true);
    }
}
