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
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _playerController.enabled = IsOwner;
        _playerAttack.enabled = IsOwner;
        _playerNetworkLife.enabled = IsOwner;
        _camera.gameObject.SetActive(IsOwner);
        _playerInput.gameObject.SetActive(IsOwner);


        transform.position = GetComponent<PlayerSpawn>().GetRandomSpawnPoint().position;

        UpdateTagLocally();
    }

    private void UpdateTagLocally()
    {
        if (IsOwner)
        {
            gameObject.tag = "Player"; // Tag local pour le propriétaire du personnage
        }
        else
        {
            gameObject.tag = "Untagged"; // Tous les autres joueurs sont vus comme "Untagged"
        }
    }
}