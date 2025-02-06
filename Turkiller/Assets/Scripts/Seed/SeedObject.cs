using Unity.Netcode;
using UnityEngine;

public class SeedObject : NetworkBehaviour
{
    public Seeds seeds;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private ulong _ownerClientId;
    private NetworkVariable<int> spriteIndex = new NetworkVariable<int>(0);
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
        spriteIndex.Value = seeds.spriteIndex;
        _spawner = spawner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

        PlayerEffects playerEffect = collision.gameObject.GetComponent<PlayerEffects>();
        if (playerEffect != null)
        {
            PlayerEffects.BonusEffect caca = new() { bonus = seeds.bonus, maxTime = seeds.bonusDuration, time = seeds.bonusDuration, intensity = seeds.bonusIntensity };
            
            playerEffect.AddEffectServerRpc(caca, playerEffect.OwnerClientId);
            playerEffect.GetComponent<PlayerInfo>().AddScore(seeds.puntos);

            NetworkObject projNetworkObject = gameObject.GetComponent<NetworkObject>();

            if (projNetworkObject != null)
            {
                _spawner.SeedCollectedServerRpc();
                projNetworkObject.Despawn(true);
            }
        }
    }
}
