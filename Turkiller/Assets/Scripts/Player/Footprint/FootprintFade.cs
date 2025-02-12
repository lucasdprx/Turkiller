using Unity.Netcode;
using UnityEngine;

public class FootprintFade : NetworkBehaviour
{
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _fadeDuration = 0.5f;
    private float _fadeTimer;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DestroyFootprint), _fadeDuration);
        }
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _fadeTimer = _fadeDuration;
    }

    private void Update()
    {
        _fadeTimer -= Time.deltaTime;

        if (_fadeTimer <= 0) return;

 
        float alpha = _fadeTimer / _fadeDuration;
        Color newColor = _spriteRenderer.color;
        newColor.a = alpha;
        _spriteRenderer.color = newColor;
    }

    private void DestroyFootprint()
    {
        if (IsServer)
        {
            NetworkObject.Despawn();
        }
    }
}
