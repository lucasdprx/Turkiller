using System;
using NUnit.Framework;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.UI;
using UnityEngine.UI;

public class PlayerNetworkLife : NetworkBehaviour
{
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private NetworkVariable<int> _maxLife = new NetworkVariable<int>(100);
    private NetworkVariable<float> _currentLife = new NetworkVariable<float>(100);

    public void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamageServerRpc(10);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _currentLife.Value = _maxLife.Value;
            _lifeSlider.maxValue = _maxLife.Value;
            _lifeSlider.value = _currentLife.Value;
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        _currentLife.Value -= damage;
        _lifeSlider.value = _currentLife.Value;

        if (_currentLife.Value <= 0)
        {
            Die();
        }
    }
    
    [ServerRpc]
    public void HealServerRpc(int heal)
    {
        _currentLife.Value += heal;
        _lifeSlider.value = _currentLife.Value;
    }

    public void Die()
    {
        if (!IsServer) return;
        Debug.Log("you are dead");
    }
    
    public bool IsAlive()
    {
        return _currentLife.Value > 0;
    }
}
