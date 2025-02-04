using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class PlayerNetworkLife : NetworkBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private Image _healthBar;
    
    private NetworkVariable<int> _currentHealth = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _currentHealth.Value = _maxHealth;
        }

        _currentHealth.OnValueChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(int oldValue, int newValue)
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount = (float)newValue / _maxHealth;
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        _currentHealth.Value = Mathf.Max(0, _currentHealth.Value - damage);
        UpdateHealthClientRpc(_currentHealth.Value);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int newHealth)
    {
        Debug.Log(newHealth);
        if (_healthBar != null)
        {
            _healthBar.fillAmount = (float)newHealth / _maxHealth;
        }
        Debug.Log(_healthBar.fillAmount);
    }
}