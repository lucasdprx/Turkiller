using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class PlayerNetworkLife : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private Image _healthBar;
    [SerializeField] private PlayerEffects _effects;

    private NetworkVariable<float> _currentHealth = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        if (IsServer) 
            _currentHealth.Value = _maxHealth;
        
        _healthBar.fillAmount = _currentHealth.Value / _maxHealth;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage, ulong targetClientId)
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            if (networkObject.OwnerClientId != targetClientId)
                continue;
            
            PlayerNetworkLife playerNetworkLife = player.GetComponent<PlayerNetworkLife>();
            playerNetworkLife._currentHealth.Value -= damage * _effects.GetEffect(Bonus.DamageTakenMultiplier).min;
            playerNetworkLife.UpdateHealthClientRpc(playerNetworkLife._currentHealth.Value);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateHealthClientRpc(float newHealth)
    {
        _healthBar.fillAmount = newHealth / _maxHealth;
    }
}
