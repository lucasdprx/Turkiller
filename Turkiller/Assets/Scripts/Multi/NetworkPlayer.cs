using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private PlayerController _playerController;
    private Camera _camera;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _camera = GetComponentInChildren<Camera>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _playerController.enabled = IsOwner;
        _camera.depth = IsOwner ? 1 : 0;
    }
}
