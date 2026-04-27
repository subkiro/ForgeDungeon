using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "InventoryItemSO", menuName = "Scriptable Objects/InventoryItemSO")]
public abstract class InventoryItemSO : ScriptableObject
{
    public string ID;
    public string DisplayName;
     public string Descripton;
    public Sprite Icon;
    [SerializeField] Rarity Rarity;
    public List<Stats> BasicStats;





    public RarityData RarityData=>GameManager.Instance.AssetScriptableData.GetRarity(Rarity);



}


[Serializable]
public struct RarityData
{
    public Rarity Rarity;
    public Color color;
}
public enum Rarity
{
Common,
Uncommon,
Rare,
Epic,
Legendary
}
