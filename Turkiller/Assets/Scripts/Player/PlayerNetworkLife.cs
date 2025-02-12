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
    [SerializeField] private GameObject _damageParticlesPrefab;
    [SerializeField] private Animator _animator;

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

        if (newValue < previousValue)
        {
            ShowDamageEffect();
            _animator.SetTrigger("TakeDamage");
        }

        if (!(newValue <= 0f) || !IsOwner)
            return;

        _deathMenu.SetActive(true);
        DieServerRpc(OwnerClientId);
    }

    private void ShowDamageEffect()
    {
        GameObject damageEffect = Instantiate(_damageParticlesPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = damageEffect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
        Destroy(damageEffect, ps.main.duration);
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

            // Recuperer le _soundPackIndex et appeler PlaySFXPain
            int soundPackIndex = player.GetComponent<PlayerController>()._soundPackIndex;
            AudioManager.Instance.PlaySFXPain(soundPackIndex);
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

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            if (networkObject.OwnerClientId != targetClientId)
                continue;

            // Récupérer le _soundPackIndex et appele PlaySFXDeath
            int soundPackIndex = player.GetComponent<PlayerController>()._soundPackIndex;
            AudioManager.Instance.PlaySFXDeath(soundPackIndex);
        }

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
