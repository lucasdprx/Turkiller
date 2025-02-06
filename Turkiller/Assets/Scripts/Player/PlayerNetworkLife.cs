using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetworkLife : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private Image _healthBar;
    [SerializeField] private PlayerEffects _effects;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _player;

    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>(
        100,
        NetworkVariableReadPermission.Everyone, // Tout le monde peut lire
        NetworkVariableWritePermission.Server // Seul le serveur peut �crire
    );

    private void Start()
    {
        _deathMenu.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            _currentHealth.Value = _maxHealth;

        _healthBar.fillAmount = _currentHealth.Value / _maxHealth;


        _currentHealth.OnValueChanged += OnHealthChanged;

    }

    public void SpawnTest()
    {
        RespawnPlayerClientRpc(OwnerClientId);
    }

    private void OnDestroy()
    {
        _currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        _healthBar.fillAmount = newValue / _maxHealth;

        if (newValue <= 0f)
        {
            if (IsOwner)
                _deathMenu.SetActive(true);

            DieServerRpc(OwnerClientId);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong targetClientId)
    {
        if (!IsServer) return;

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            if (networkObject.OwnerClientId != targetClientId)
                continue;

            PlayerNetworkLife playerNetworkLife = player.GetComponent<PlayerNetworkLife>();
            playerNetworkLife._currentHealth.Value -= damage * playerNetworkLife._effects.GetEffect(Bonus.DamageTakenMultiplier).min + 10;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DieClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out var player))
        {
            _healthBar.transform.parent.gameObject.SetActive(false);
            player.PlayerObject.GetComponent<PlayerAttack>().enabled = false;
            player.PlayerObject.GetComponent<PlayerController>().enabled = false;
            player.PlayerObject.GetComponentInChildren<Collider2D>().enabled = false;
            player.PlayerObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DieServerRpc(ulong targetClientId)
    {
        DieClientRpc(targetClientId);
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RespawnPlayerClientRpc(ulong clientId)
    {
        if (clientId != OwnerClientId) return; 

        _deathMenu.SetActive(false);
        _healthBar.transform.parent.gameObject.SetActive(true);
        NetworkObject playerPrefab = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        playerPrefab.transform.position = new Vector3(0f, 1f, 0f);
        playerPrefab.GetComponent<PlayerAttack>().enabled = true;
        playerPrefab.GetComponentInChildren<Collider2D>().enabled = true;
        playerPrefab.GetComponent<PlayerController>().enabled = true;
        _player.GetComponentInChildren<SpriteRenderer>().enabled = true;



        if (IsServer) _currentHealth.Value = _maxHealth;
        _player.SetActive(true);
    }

}
