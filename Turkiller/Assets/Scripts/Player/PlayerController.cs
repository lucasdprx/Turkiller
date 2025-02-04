using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    private Vector2 _moveDirection;
    private Rigidbody2D _rb;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void GetInputMovement(InputAction.CallbackContext ctx)
    {
        _moveDirection = ctx.ReadValue<Vector2>();
        
        if (ctx.canceled)
            _rb.linearVelocity = Vector2.zero;
    }
    private void Movement()
    {
        if (_rb == null || _moveDirection == Vector2.zero) return;
        
        _rb.linearVelocity = _moveDirection * _moveSpeed;
    }
    private void Update()
    {
        Movement();
    }
}
