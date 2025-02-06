using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Seed", menuName = "ScriptableObjects/CreateSeed", order = 1)]
public class Seeds : ScriptableObject
{
    public string SeedName;
    public int spriteIndex;
    public Bonus bonus;
    public float bonusDuration;
    public float bonusIntensity;
    public int puntos;
}

[Serializable] public enum Bonus
{
    None,
    DamageTakenMultiplier,
    AttackSpeed,
    MoveSpeed,
    DamageMultiplier
}