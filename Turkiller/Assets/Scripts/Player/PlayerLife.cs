using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private Image _lifeImage;
    [SerializeField] private int _maxLife = 100;
    private float _currentLife;

    private void Start()
    {
        _currentLife = _maxLife;
        _lifeImage.fillAmount = _currentLife / _maxLife;
    }

    public void TakeDamage(int damage)
    {
        _currentLife -= damage;
        _lifeImage.fillAmount = _currentLife / _maxLife;

        if (_currentLife <= 0)
        {
            Debug.Log("Player Dead");
        }
    }
    
    public void Heal(int heal)
    {
        _currentLife += heal;
        _lifeImage.fillAmount = _currentLife / _maxLife;
    }
    
    public bool IsAlive()
    {
        return _currentLife > 0;
    }
}
