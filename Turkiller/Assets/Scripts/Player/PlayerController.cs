using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    private Vector2 _moveDirection;
    private Rigidbody2D _rb;
    [SerializeField] private PlayerEffects _effects;



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

        _rb.linearVelocity = _moveDirection * _moveSpeed * _effects.GetEffect(Bonus.MoveSpeed).max;
        //Vector3 dir = _moveDirection * _moveSpeed;
        //transform.position += dir;
    }
    private void Update()
    {
        Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
    }
}
