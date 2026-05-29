using UnityEngine;

public class RawResourceINV
{
    public RawResourceSO RawResourceScriptable { get; private set; }
    public double Quantity { get; set; }

    public Rarity Rarity => RawResourceScriptable.BaseRarity;

    public double BuyPrice => RawResourceScriptable.BuyPrice;

    public double SellPrice => RawResourceScriptable.BaseSellPrice;

    public RawResourceINV(RawResourceSO rawResource, double quantity)
    {
        RawResourceScriptable = rawResource;
        Quantity = quantity;
    }

    public void AdjustQuantity(int change)
    {
        Quantity += change;
    }
}