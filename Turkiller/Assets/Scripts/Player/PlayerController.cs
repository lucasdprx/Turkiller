using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private PlayerEffects _effects;
    [SerializeField] private GameObject _footprintPrefab;
    [SerializeField] private float _footstepDistance = 0.3f;
    [SerializeField] private float _footstepInterval = 0.2f;

    private Vector2 _moveDirection;
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private Transform _spritePlayer;
    private Camera _camera;
    private Transform _transform;
    private PlayerAttack _playerAttack;

    private float _lastFootstepTime;
    private bool _isLeftFoot = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponentInChildren<PlayerInput>();
        _spritePlayer = GetComponentInChildren<SpriteRenderer>().transform;
        _camera = GetComponentInChildren<Camera>();
        _playerAttack = GetComponent<PlayerAttack>();
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

        bool isDistance = _playerAttack.GetIsDistance();
        _spritePlayer.Rotate(0, 0, isDistance ? 90 : -90);
    }

    public PlayerEffects Effects() => _effects;

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
        if (_rb == null) return;

        if (_moveDirection == Vector2.zero) return;

        _rb.linearVelocity = _moveDirection * (_moveSpeed * _effects.GetEffect(Bonus.MoveSpeed).max);

        if (Time.time - _lastFootstepTime > _footstepInterval)
        {
            SpawnFootprint();
            _lastFootstepTime = Time.time;
        }
    }

    private void SpawnFootprint()
    {
        float offsetDirection = _isLeftFoot ? -1 : 1;
        _isLeftFoot = !_isLeftFoot;

        Vector3 footprintPosition = _transform.position + (_spritePlayer.right * _footstepDistance * offsetDirection);
        Quaternion footprintRotation = _spritePlayer.rotation;

        if (IsServer)
        {
            GameObject footprint = Instantiate(_footprintPrefab, footprintPosition, footprintRotation);
            footprint.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            SpawnFootprintServerRpc(footprintPosition, footprintRotation);
        }
    }

    [ServerRpc]
    private void SpawnFootprintServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject footprint = Instantiate(_footprintPrefab, position, rotation);
        footprint.GetComponent<NetworkObject>().Spawn();
    }

}
