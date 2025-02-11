using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private bool _isDistanceAttack;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerController = GetComponent<PlayerController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_isAttacking)
            Attack();
    }
    private IEnumerator TimeMeleeAttack()
    {
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
    }

    public bool GetIsDistance()
    {
        return _isDistanceAttack;
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

    private void ChangeStateAttack()
    {
        _isDistanceAttack = !_isDistanceAttack;

        _spriteRenderer.transform.Rotate(0, 0, 180);
    }

    private void Attack()
    {
        if (_isDistanceAttack && _attackTimer >= _distanceAttackSpeed / _playerController.Effects().GetEffect(Bonus.AttackSpeed).max)
        {
            _attackTimer = 0;

            Vector3 position = _spawnPoint.position;
            Vector3 dir = GetMousePosition(_camera) - position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion direction = Quaternion.AngleAxis(angle, new Vector3(0, _spawnPoint.rotation.y, 1));

            ulong ownerClientId = GetComponent<NetworkObject>().OwnerClientId;
            _rb.AddForce(-dir.normalized * _recoilDistanceAttack, ForceMode2D.Impulse);
            RequestDistanceAttackServerRpc(ownerClientId, position, direction);
        }

        else if (!_isDistanceAttack && _attackTimer >= _MeleeAttackSpeed)
        {
            _attackTimer = 0;
            Vector3 dir = (GetMousePosition(_camera) - _spawnPoint.position).normalized;
            _rb.AddForce(dir * _recoilMeleeAttack, ForceMode2D.Impulse);
            StartCoroutine(TimeMeleeAttack());
        }
    }
    
    public void LeftClick(InputAction.CallbackContext context)
    {
        if(context.performed && IsOwner)
        {
            _isAttacking = true;
        }
        
        else if(context.canceled && IsOwner)
        {
            _isAttacking = false;
        }
    }

    public void RightClick(InputAction.CallbackContext context)
    {
        if(context.performed)
            ChangeStateAttack();
    }

}
