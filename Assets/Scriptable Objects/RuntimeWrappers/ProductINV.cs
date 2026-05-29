using System.Collections.Generic;
using UnityEngine;

public class ProductINV
{
    public EndProductSO EndProductSO { get; private set; }
    public double Quality { get; private set; }

    //The calculated stats of the object
    public Dictionary<string, double> Parameters { get; private set; } = new();

    public double SellPrice => PriceCalculatorProcess.CalculateSellPrice(
        InventoryItemTypes.Product,
        EndProductSO.BaseSellPrice,
        Quality);

    public Rarity Rarity =>
        RarityCalculatorProcess.CalculateRarity(EndProductSO.RarityData.Rarity, Quality);

    // Optional modifiers/bonuses that can be added/removed at any time
    //If a parameter is added as optional and there is no Base Parameter value in the scriptable it will be ignored. e.g. if slashing bonus is added on a piercing weapon.
    //TODO: make them reactive; on change re-calculate EndParameters
    public Dictionary<string, double> OptionalParameterModifierAdditive { get; set; } = new();
    public Dictionary<string, double> OptionalParameterModifierMultiplicative { get; set; } = new();

    public ProductINV(EndProductSO endProductSO, double quality)
    {
        EndProductSO = endProductSO;
        Quality = quality;
        CalculateEndParameters();
    }

    private void CalculateEndParameters()
    {
        Parameters.Clear();
        if (EndProductSO.BaseParametersList == null) return;

        foreach (var param in EndProductSO.BaseParametersList)
        {
            // Value = BaseValue + 0.01% of Quality  (0.0001 * Quality)
            double calculatedValue = param.Value + (Quality * 0.0001);
            Parameters[param.Key] = calculatedValue;
        }
    }
}
