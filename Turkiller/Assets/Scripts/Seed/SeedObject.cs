using System;
using Unity.Netcode;
using UnityEngine;

public class SeedObject : NetworkBehaviour
{
    public Seeds seeds;
    public SpriteRenderer spriteRenderer;

    private ulong _ownerClientId;

    public void SetOwner(ulong ownerId)
    {
        _ownerClientId = ownerId;
    }

    public void Init(Seeds newSeed)
    {
        seeds = newSeed;
        spriteRenderer.sprite = seeds.sprite;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("a");
        if (!IsServer) return;

        NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

        Debug.Log("Seed hit something");
        PlayerController playerNetworkLife = collision.gameObject.GetComponent<PlayerController>();
        if (playerNetworkLife != null)
        {
            print("c");
            playerNetworkLife.AddEffectServerRpc(seeds);
            NetworkObject projNetworkObject = gameObject.GetComponent<NetworkObject>();
            if (projNetworkObject != null)
            {
                print("d");
                projNetworkObject.Despawn(true);
            }
        }
    }
}
