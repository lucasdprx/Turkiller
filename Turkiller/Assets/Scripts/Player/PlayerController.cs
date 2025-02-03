using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    private Vector2 _moveDirection;
    private Rigidbody2D _rb;

    [Serializable]
    public struct BonusEffect
    {
        public Bonus bonus;
        public float time;
        public float intensity;
    }

    public List<BonusEffect> effects = new List<BonusEffect>();


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
        HandleEffect();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddEffectServerRpc(Seeds seed)
    {
        BonusEffect effect = new BonusEffect();
        effect.time = seed.bonusDuration;
        effect.intensity = seed.bonusIntensity;
        effect.bonus = seed.bonus;
        effects.Add(effect);
    }

    public void HandleEffect()
    {
        for(int i = 0; i < effects.Count; i++)
        {
            BonusEffect tmpEffect = effects[i];

            tmpEffect.time -= Time.deltaTime;

            effects[i] = tmpEffect;

            if(tmpEffect.time <= 0)
            {
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
    }
}
