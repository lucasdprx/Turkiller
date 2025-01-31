using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Camera _camera;

    private void Update()
    {
        if (IsOwner && Input.GetMouseButtonDown(0))
        {
            RequestAttackServerRpc(OwnerClientId);
        }
    }

    [ServerRpc]
    private void RequestAttackServerRpc(ulong ownerClientId)
    {
        GameObject projectile = Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

        // Rotate
        Vector3 dir = mousePos - projectile.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, projectile.transform.rotation.y, 1));

        NetworkObject networkObject = projectile.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            projectile.GetComponent<ProjectileComponent>().SetOwner(ownerClientId); // Assignation de l'ID
            networkObject.Spawn(true);
        }
    }
}
