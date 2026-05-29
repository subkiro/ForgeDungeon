using UnityEngine;

public class MaterialINV
{
    public MaterialSO MaterialScriptable { get; private set; }
    public double Quality { get; private set; }

    public Rarity Rarity =>
    RarityCalculatorProcess.CalculateRarity(MaterialScriptable.RarityData.Rarity, Quality);

    public double SellPrice => PriceCalculatorProcess.CalculateSellPrice(
        InventoryItemTypes.Material,
        MaterialScriptable.BaseSellPrice,
        Quality);



    public MaterialINV(MaterialSO materialSO, double quality)
    {
        MaterialScriptable = materialSO;
        Quality = quality;
    }
}
