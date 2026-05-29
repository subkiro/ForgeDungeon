using System;
using UnityEngine;

[Serializable]
public struct RarityData
{
    public Rarity Rarity;
    public Color color;
    public double QualityThreshold;
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
