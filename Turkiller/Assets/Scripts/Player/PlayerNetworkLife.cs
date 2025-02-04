using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class PlayerNetworkLife : NetworkBehaviour
{
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private NetworkVariable<int> _maxLife = new NetworkVariable<int>(100);
    private NetworkVariable<float> _currentLife = new NetworkVariable<float>(100);
    [SerializeField] private PlayerEffects _effects;

    private void Start()
    {
        if (IsServer)
        {
            _currentLife.Value = _maxLife.Value;
        }
        _lifeSlider.maxValue = _maxLife.Value;
        _lifeSlider.value = _currentLife.Value;
        _currentLife.OnValueChanged += OnLifeChanged;
    }

    private void OnLifeChanged(float oldLife, float newLife)
    {
        _lifeSlider.value = newLife;
        if (newLife <= 0)
        {
            Die();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _lifeSlider.maxValue = _maxLife.Value;
        _lifeSlider.value = _currentLife.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        _currentLife.Value -= damage * _effects.GetEffect(Bonus.DamageTakenMultiplier).min;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(int heal)
    {
        _currentLife.Value += heal;
        if (_currentLife.Value > _maxLife.Value)
        {
            _currentLife.Value = _maxLife.Value;
        }
    }

    private void Die()
    {
        if (!IsServer) return;
        Debug.Log("Player Dead");
    }
    
    public bool IsAlive()
    {
        return _currentLife.Value > 0;
    }
}