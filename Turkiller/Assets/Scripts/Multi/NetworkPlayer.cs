using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    private PlayerController _playerController;
    private PlayerAttack _playerAttack;
    private Camera _camera;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAttack = GetComponent<PlayerAttack>();
        _camera = GetComponentInChildren<Camera>();
        _playerInput = GetComponentInChildren<PlayerInput>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _playerController.enabled = IsOwner;
        _playerAttack.enabled = IsOwner;
        _camera.gameObject.SetActive(IsOwner);
        _playerInput.gameObject.SetActive(IsOwner);
    }
}
