using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerEffects : NetworkBehaviour
{

    [Serializable]
    public struct BonusEffect : INetworkSerializable, IEquatable<BonusEffect>
    {
        public float time;
        public float maxTime;
        public float intensity;
        public Bonus bonus;

        

        // S�rialisation pour la synchronisation r�seau
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref time);
            serializer.SerializeValue(ref maxTime);
            serializer.SerializeValue(ref intensity);
            serializer.SerializeValue(ref bonus);
        }

        // Impl�mentation de IEquatable pour NetworkList
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

    public GameObject effectUIPrefab;
    private Transform _effectParent;

    private List<BonusEffect> effects = new List<BonusEffect>();

    public List<BonusEffect> testEffects = new();

    

    private void Start()
    {
        _effectParent = ReferenceManager.instance.parentsEffectUI;
    }

    


    private void Update()
    {
        DecreaseEffect();
        testEffects.Clear();
        foreach (var effect in effects)
        {
            testEffects.Add(effect);
        }

    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void AddEffectServerRpc(BonusEffect seed, ulong id)
    {
        CreateUIClientRpc(id, seed); 
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void CreateUIClientRpc(ulong id, BonusEffect bonus)
    {
        bool passed = true;
        foreach (var item in effects)
        {
            if (item.bonus == bonus.bonus)
                passed = false;
        }
        if (passed)
        {
            if (id == NetworkManager.Singleton.LocalClientId)
            {
                Instantiate(effectUIPrefab, _effectParent).GetComponent<EffectUI>().Init(this, bonus.bonus);
            }
        }         
        effects.Add(bonus);
    }

    
    public void DecreaseEffect()
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

    public (float time, float max) HighestTimer(Bonus bonus)
    {
        float highestTime = 0;
        float highestMax = 0;

        for (int i = 0; i < effects.Count; i++)
        {
            if (bonus == effects[i].bonus && effects[i].time > highestTime)
            {
                highestTime = effects[i].time;
                highestMax = effects[i].maxTime;
            }
        }
        return (highestTime, highestMax);
    }

    public (float min, float max) GetEffect(Bonus bonus)
    {
        (float min, float max) = (1, 1);

        for (int i = 0;i < effects.Count;i++)
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
