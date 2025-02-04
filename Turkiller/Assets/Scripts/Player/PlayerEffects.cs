using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerEffects : NetworkBehaviour
{

    [Serializable]
    public struct BonusEffect : INetworkSerializable, IEquatable<BonusEffect>
    {
        public float time;
        public float intensity;
        public Bonus bonus;

        // Sérialisation pour la synchronisation réseau
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref time);
            serializer.SerializeValue(ref intensity);
            serializer.SerializeValue(ref bonus);
        }

        // Implémentation de IEquatable pour NetworkList
        public bool Equals(BonusEffect other)
        {
            return time == other.time && intensity == other.intensity && bonus == other.bonus;
        }

        public override bool Equals(object obj)
        {
            return obj is BonusEffect other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(time, intensity, bonus);
        }
    }

    private NetworkList<BonusEffect> effects = new NetworkList<BonusEffect>();


    private void Update()
    {
        DecreaseEffectServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddEffectServerRpc(BonusEffect seed)
    {
        effects.Add(seed);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DecreaseEffectServerRpc()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            BonusEffect tmpEffect = effects[i];

            tmpEffect.time -= Time.deltaTime;

            effects[i] = tmpEffect;

            if (tmpEffect.time <= 0)
            {
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    public (float min, float max) GetEffect(Bonus bonus)
    {
        (float min, float max) = (1, 1);

        for(int i = 0;i < effects.Count;i++)
        {
            if(effects[i].bonus == bonus)
            {
                if (effects[i].intensity < min)
                {
                    min = effects[i].intensity;
                }
                else if (effects[i].intensity > max)
                {
                    max = effects[i].intensity;
                }
            }
        }

        return (min, max);
    }
}
