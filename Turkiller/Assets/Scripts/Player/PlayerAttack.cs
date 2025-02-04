using Unity.Netcode;
using UnityEngine;
public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Camera _camera;

    private void Update()
    {
        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMousePosition();
            Vector3 dir = mousePos - _spawnPoint.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion direction = Quaternion.AngleAxis(angle, new Vector3(0, _spawnPoint.rotation.y, 1));
            RequestAttackServerRpc(OwnerClientId, _spawnPoint.position, direction);
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
        
        projectileComponent.SetVelocity();
        
        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        if (networkObject == null)
            return;
        
        projectileComponent.SetOwner(ownerClientId); // Assignation de l'ID
        networkObject.Spawn(true);
    }
}
