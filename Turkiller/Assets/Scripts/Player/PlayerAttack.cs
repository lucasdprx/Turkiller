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

    private float _attackTimer;

    private void Update()
    {
        _attackTimer += Time.deltaTime;
        if (IsOwner && Input.GetMouseButton(0) && _attackTimer >= _distanceAttackSpeed)
        {
            _attackTimer = 0;
            Vector3 mousePos = GetMousePosition();
            Vector3 dir = mousePos - _spawnPoint.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion direction = Quaternion.AngleAxis(angle, new Vector3(0, _spawnPoint.rotation.y, 1));
            ulong ownerClientId = GetComponent<NetworkObject>().OwnerClientId;
            RequestAttackServerRpc(ownerClientId, _spawnPoint.position, direction);
        }
    }
    private Vector3 GetMousePosition()
    {
        return _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    [ServerRpc]
    private void RequestAttackServerRpc(ulong ownerClientId, Vector3 spawnPoint, Quaternion direction)
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
