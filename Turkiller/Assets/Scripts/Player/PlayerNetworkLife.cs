// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.UI;
// public class PlayerNetworkLife : NetworkBehaviour
// {
//     [SerializeField] private Image _lifeImage;
//     [SerializeField] private NetworkVariable<int> _maxLife = new NetworkVariable<int>(100);
//     private readonly NetworkVariable<float> _currentLife = new NetworkVariable<float>(100f);
//
//     private void Start()
//     {
//         if (IsServer)
//         {
//             _currentLife.Value = _maxLife.Value;
//         }
//         _lifeImage.fillAmount = _currentLife.Value / _maxLife.Value;
//         _currentLife.OnValueChanged += OnLifeChangedServerRpc;
//     }
//     
//     [ServerRpc]
//     private void OnLifeChangedServerRpc(float oldLife, float newLife)
//     {
//         _lifeImage.fillAmount = newLife / _maxLife.Value;
//         if (newLife <= 0)
//         {
//             Die();
//         }
//     }
//
//     public override void OnNetworkSpawn()
//     {
//         base.OnNetworkSpawn();
//         _currentLife.Value = _maxLife.Value;
//         _lifeImage.fillAmount = _currentLife.Value / _maxLife.Value;
//     }
//
//     [ServerRpc(RequireOwnership = false)]
//     public void TakeDamageServerRpc(int damage)
//     {
//         _currentLife.Value -= damage;
//     }
//     
//     [ServerRpc(RequireOwnership = false)]
//     public void HealServerRpc(int heal)
//     {
//         _currentLife.Value += heal;
//         if (_currentLife.Value > _maxLife.Value)
//         {
//             _currentLife.Value = _maxLife.Value;
//         }
//     }
//
//     private void Die()
//     {
//         if (!IsServer) return;
//         Debug.Log("Player Dead");
//     }
//     
//     public bool IsAlive()
//     {
//         return _currentLife.Value > 0;
//     }
// }