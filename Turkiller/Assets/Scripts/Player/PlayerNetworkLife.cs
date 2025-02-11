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
    [SerializeField] private GameObject _bloodParticlesPrefab;


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
            Debug.Log(damage * playerNetworkLife._effects.GetEffect(Bonus.DamageTakenMultiplier).min);

            playerNetworkLife._currentHealth.Value -= damage * playerNetworkLife._effects.GetEffect(Bonus.DamageTakenMultiplier).max;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void DieClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out NetworkClient player))
        {
            Transform playerTransform = player.PlayerObject.transform;

            GameObject bloodEffect = Instantiate(_bloodParticlesPrefab, playerTransform.position, Quaternion.identity);
            ParticleSystem ps = bloodEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            Destroy(bloodEffect, ps.main.duration);

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
        _currentHealth.Value = _maxHealth;
        DieClientRpc(targetClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void RespawnPlayerClientRpc(ulong clientId)
    {
        if (clientId != OwnerClientId) return;

        _deathMenu.SetActive(false);
        
        NetworkObject playerPrefab = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        playerPrefab.transform.position = playerPrefab.GetComponent<PlayerSpawn>().GetRandomSpawnPoint().position;
        playerPrefab.GetComponent<PlayerAttack>().enabled = true;
        playerPrefab.GetComponentInChildren<Collider2D>().enabled = true;
        playerPrefab.GetComponent<PlayerController>().enabled = true;
        _player.GetComponentInChildren<SpriteRenderer>().enabled = true;
        _healthBar.transform.parent.gameObject.SetActive(true);
        _player.SetActive(true);
    }
}
