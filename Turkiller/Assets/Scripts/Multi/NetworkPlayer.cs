using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    private PlayerController _playerController;
    private PlayerAttack _playerAttack;
    private Camera _camera;
    private PlayerInput _playerInput;
    private PlayerNetworkLife _playerNetworkLife;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerNetworkLife = GetComponent<PlayerNetworkLife>();
        _camera = GetComponentInChildren<Camera>();
        _playerInput = GetComponentInChildren<PlayerInput>();
        transform.position = GetComponent<PlayerSpawn>().GetRandomSpawnPoint().position;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _playerController.enabled = IsOwner;
        _playerAttack.enabled = IsOwner;
        _playerNetworkLife.enabled = IsOwner;
        _camera.gameObject.SetActive(IsOwner);
        _playerInput.gameObject.SetActive(IsOwner);
        

        UpdateTagLocally();
    }

    private void UpdateTagLocally()
    {
        gameObject.tag = IsOwner ? "Player" : "Untagged";
    }
}