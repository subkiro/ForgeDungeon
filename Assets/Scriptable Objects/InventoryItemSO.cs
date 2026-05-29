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
    public double BaseSellPrice;
    public List<string> Categories;

    //Reorganize/Remove
    [Header("IGNORE RARITY, BASE STATS, COST")]
    [SerializeField] Rarity Rarity;
    public List<Stats> BasicStats;

    public Stats Cost = new Stats{Stat_Type=StatType.Coin};




    public RarityData RarityData=>GameManager.Instance.AssetScriptableData.GetRarity(Rarity);



}



