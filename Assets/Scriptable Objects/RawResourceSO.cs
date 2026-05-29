using UnityEngine;

[CreateAssetMenu(fileName = "RawResource", menuName = "InventoryItems/RawResourceSO")]
public class RawResourceSO : InventoryItemSO
{
    [Header("Raw Resource Variables")]
    public Rarity BaseRarity;
    public double BuyPrice;
}
