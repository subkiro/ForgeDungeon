using UnityEngine;

public class RarityCalculatorProcess
{
    //Used in End Products to determine their final stats
    //
    public static double BonusParametersPerQualityTier = 0.8;


    /// <summary>
    /// Calculates a rarity based on a base rarity and quality.
    /// (The exact logic is to be defined; the return type uses the existing ItemRarities enum.)
    /// </summary>
    public static Rarity CalculateRarity(Rarity baseRarity, double quality)
    {
        // Placeholder – implement as needed
        return baseRarity;
    }

    public static int ConvertRarityToInt(Rarity baseRarity)
    {
        // Placeholder – implement as needed
        return 1;
    }

    public static double GetRarityParameterMultiplier(Rarity itemRarity)
    {
        return GetRarityParameterMultiplier(ConvertRarityToInt(itemRarity));
    }
    public static double GetRarityParameterMultiplier(int RarityInt)
    {
        return 1 + (double)RarityInt * BonusParametersPerQualityTier;
    }
}
