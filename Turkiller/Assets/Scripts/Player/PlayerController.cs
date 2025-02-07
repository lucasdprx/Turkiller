using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private PlayerEffects _effects;
    
    private Vector2 _moveDirection;
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private Transform _spritePlayer;
    private Camera _camera;
    private Transform _transform;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponentInChildren<PlayerInput>();
        _spritePlayer = GetComponentInChildren<SpriteRenderer>().transform;
        _camera = GetComponentInChildren<Camera>();
        _transform = transform;
    }
    
    private void Update()
    {
        Movement();
        if (!IsOwner) return;
        Vector3 dir = PlayerAttack.GetMousePosition(_camera) - _transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion direction = Quaternion.AngleAxis(angle, new Vector3(0, _transform.rotation.y, 1));
        _spritePlayer.rotation = direction;
        _spritePlayer.Rotate(0, 0, -90);
    }

    public void GetInputMovement(InputAction.CallbackContext ctx)
    {
        _moveDirection = ctx.ReadValue<Vector2>();
        
        if (ctx.canceled)
            _rb.linearVelocity = Vector2.zero;
    }

    public void FreezeInput(bool value)
    {
        _playerInput.enabled = !value;
    }
    private void Movement()
    {
        if (_rb == null || _moveDirection == Vector2.zero) return;

        _rb.linearVelocity = _moveDirection * (_moveSpeed * _effects.GetEffect(Bonus.MoveSpeed).max);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
    }
}
