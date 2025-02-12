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

    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>(100);

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

    public override void OnDestroy()
    {
        _currentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float previousValue, float newValue)
    {
        _healthBar.fillAmount = newValue / _maxHealth;

        if (!(newValue <= 0f) || !IsOwner)
            return;
        
        _deathMenu.SetActive(true);
        DieServerRpc(OwnerClientId);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong targetClientId, ulong attackerClientId) 
    {
        if (!IsServer) return;

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            if (networkObject.OwnerClientId != targetClientId)
                continue;

            PlayerNetworkLife playerNetworkLife = player.GetComponent<PlayerNetworkLife>();

            playerNetworkLife._currentHealth.Value -= damage * playerNetworkLife._effects.GetEffect(Bonus.DamageTakenMultiplier).max;

            if (playerNetworkLife._currentHealth.Value <= 0)
            {
                PlayerInfo attacker = NetworkManager.Singleton.ConnectedClients[attackerClientId].PlayerObject.GetComponent<PlayerInfo>();
                attacker.AddScore(30);
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DieClientRpc(ulong targetClientId)
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out NetworkClient player))
        {
            _healthBar.transform.parent.gameObject.SetActive(false);
            PlayerInfo playerInfo = player.PlayerObject.GetComponent<PlayerInfo>();
            player.PlayerObject.GetComponent<PlayerAttack>().enabled = false;
            player.PlayerObject.GetComponent<PlayerController>().enabled = false;
            playerInfo.circleCollider.enabled = false;
            playerInfo._bodyPlayer.SetActive(false);
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DieServerRpc(ulong targetClientId)
    {
        _currentHealth.Value = _maxHealth;
        DieClientRpc(targetClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RespawnPlayerClientRpc(ulong clientId)
    {
        if (clientId != OwnerClientId) return;

        _deathMenu.SetActive(false);
        
        NetworkObject playerPrefab = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        PlayerInfo playerInfo = playerPrefab.GetComponent<PlayerInfo>();
        playerPrefab.transform.position = playerPrefab.GetComponent<PlayerSpawn>().GetRandomSpawnPoint().position;
        playerInfo.circleCollider.enabled = true;
        playerInfo._bodyPlayer.SetActive(true);
        playerPrefab.GetComponent<PlayerController>().enabled = true;
        playerPrefab.GetComponent<PlayerAttack>().enabled = true;
        _healthBar.transform.parent.gameObject.SetActive(true);
        _player.SetActive(true);
    }
}
