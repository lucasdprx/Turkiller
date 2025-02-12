using Unity.Netcode;
using UnityEngine;

public class SeedObject : NetworkBehaviour
{
    public Seeds seeds;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private ulong _ownerClientId;
    private NetworkVariable<int> spriteIndex = new NetworkVariable<int>();
    private SeedSpawner _spawner;

    public override void OnNetworkSpawn()
    {
        
        spriteIndex.OnValueChanged += OnSpriteChanged;
        OnSpriteChanged(0, spriteIndex.Value);
    }

    private void OnSpriteChanged(int oldValue, int newValue)
    {
        spriteRenderer.sprite = sprites[newValue];
    }

    public void SetOwner(ulong ownerId)
    {
        _ownerClientId = ownerId;
    }

    public void Init(Seeds newSeed, SeedSpawner spawner)
    {
        seeds = newSeed;
        spriteIndex = new(seeds.spriteIndex);
        _spawner = spawner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        PlayerEffects playerEffect = collision.gameObject.GetComponentInParent<PlayerEffects>();
        if (playerEffect == null)
            return;
        
        PlayerEffects.BonusEffect bonusEffect = new PlayerEffects.BonusEffect { bonus = seeds.bonus, maxTime = seeds.bonusDuration, time = seeds.bonusDuration, intensity = seeds.bonusIntensity };
            
        playerEffect.AddEffectServerRpc(bonusEffect, playerEffect.OwnerClientId);
        playerEffect.GetComponent<PlayerInfo>().AddScore(seeds.puntos);

        NetworkObject projNetworkObject = gameObject.GetComponent<NetworkObject>();

        if (projNetworkObject == null)
            return;
        
        _spawner.SeedCollectedServerRpc(transform.position);

        if (!projNetworkObject.IsSpawned)
            return;
        projNetworkObject.Despawn();
    }
}
