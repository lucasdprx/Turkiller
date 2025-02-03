using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Seed", menuName = "ScriptableObjects/CreateSeed", order = 1)]
public class Seeds : ScriptableObject
{
    public string SeedName;
    public Sprite sprite;
    public Bonus bonus;
    public float bonusDuration;
    public float bonusIntensity;
}

[Serializable] public enum Bonus
{
    invincibility,
    attackSpeed,
    moveSpeed,
    damage
}