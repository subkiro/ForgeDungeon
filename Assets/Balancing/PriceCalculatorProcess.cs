using UnityEngine;

public static class PriceCalculatorProcess
{
    private static double MaterialPriceMultiplier = 1.2;
    private static double ProductPriceMultiplier = 1.5;

    public static double CalculateSellPrice(
        InventoryItemTypes inventoryType,
        double basePrice,
        double quality,
        double productPriceMultiplier = 0)
    {
        // Method intentionally left empty for now.
        // Will be implemented later.
        return 0.0;
    }
}
