using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEditor;
using UnityEngine;

public static class Constants 
{

}

[Serializable]
public struct GameConstants
{

    public BoardItem BoardItemsSettings;
    public PlayerInitSettings PlayerInitSettings;
    [GUIColor("green"),BoxGroup("Revive")] public int ReviveCost;
    [GUIColor("green"), BoxGroup("Revive")] public int ReviveExtraTime;
    public int AFKduration, AFKMergeSuggestionCycleInterval;
    public int RestartStageCost;
    public bool UseAds;
    public bool UseLeaderboard;
    public bool UseAnalytics;
    [GUIColor("orange")]
    public bool UseSecondChance;
    [GUIColor("yellow")]
    public bool UseGarage;
    [GUIColor("yellow")]
    public bool UseStar;
    [ShowIf("UseSecondChance"), GUIColor("orange")]
    public int SecondChanceCost;
    public AdsSettings AdsSettings;
    public BoosterSettings BoosterSettings;
    public int LeaderboardInitLevelIndex;
    public StepSettings StepSettings;


    public SpinnerSettings SpinnerSettings;

}



[Serializable]
public struct BoardItem
{
    public float RubberRadius;
    public float RubberArcRadius;
    public float DepthHeight(int depth) => (RubberRadius * 2 * depth)+RubberRadius;
    public Color BlinkOutlineColor;

}

[Serializable]
public struct StepSettings
{
    //Settings for Steps
    public StepLimit[] StepConstants;

}
[Serializable]
public struct StepLimit
{
    public int From;
    public int To;
}


[Serializable]
public struct PlayerInitSettings {
    //Player Initial Data
    public int Coins;
    public int Score;
    public int Diamonds;
    [InlineButton("Play"),GUIColor("orange")]
    public int Level;
    public int LoopFromIndex;
    public bool isRandomLooping;

    void Play() { 
        PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
        EditorApplication.EnterPlaymode();
#endif
    }
}






[Serializable]
public struct BoosterSettings
{
    [InfoBox("Prices are per 1 booster, you need to add amount in Purchase message prefab, will be multiplied")]
    [FoldoutGroup("Size Up Booster")] public int SizeUpBoosterLvlRequirement;
    [FoldoutGroup("Size Up Booster")] public int SizeUpBoosterPrice;
    [FoldoutGroup("Size Up Booster")] public int SizeUpDuration;
    [FoldoutGroup("Size Up Booster")] public float SizeupBoosterScaleMultyplier;
    [FoldoutGroup("Size Up Booster")] public string SizeUpBoosterDescription;
    [FoldoutGroup("Size Up Booster")] public string SizeUpBoosterTitle;
    [FoldoutGroup("Size Up Booster")] public string SizeUpBoosterIntroduction;

    [InfoBox("Prices are per 1 booster, you need to add amount in Purchase message prefab, will be multiplied")]
    [FoldoutGroup("Freeze Time Booster")] public int FreezeTimeBoosterLvlRequirement;
    [FoldoutGroup("Freeze Time Booster")] public int FreezeTimeBoosterPrice;
    [FoldoutGroup("Freeze Time Booster")] public int FreezeTimeDuration;
    [FoldoutGroup("Freeze Time Booster")] public string FreezeTimeBoosterDescription;
    [FoldoutGroup("Freeze Time Booster")] public string FreezeTimeBoosterTitle;
    [FoldoutGroup("Freeze Time Booster")] public string FreezeTimeBoosterIntroduction;

    [InfoBox("Prices are per 1 booster, you need to add amount in Purchase message prefab, will be multiplied")]
    [FoldoutGroup("Navigation Booster")] public int NavigationBoosterLvlRequirement;
    [FoldoutGroup("Navigation Booster")] public int NavigationBoosterPrice;
    [FoldoutGroup("Navigation Booster")] public string NavigationBoosterDescription;
    [FoldoutGroup("Navigation Booster")] public string NavigationBoosterTitle;
    [FoldoutGroup("Navigation Booster")] public string NavigationBoosterIntroduction;
    [FoldoutGroup("Navigation Booster")] public int NavigationDuration;

    [GUIColor("yellow")] public int AmountPerPurchase, AmountPerAds, GiveFreeForTutorial;

    public int Duration(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpDuration,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeDuration,
        RewardType.NAVIGATIONBOOSTER => NavigationDuration,
        _ => default

    };

    public int Prize(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpBoosterPrice,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeBoosterPrice,
        RewardType.NAVIGATIONBOOSTER => NavigationBoosterPrice,
        _ => default

    };
    public string Description(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpBoosterDescription,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeBoosterDescription,
        RewardType.NAVIGATIONBOOSTER => NavigationBoosterDescription,
        _ => default
    };
    public string Title(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpBoosterTitle,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeBoosterTitle,
        RewardType.NAVIGATIONBOOSTER => NavigationBoosterTitle,
        _ => default
    };
    public string Introduction(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpBoosterIntroduction,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeBoosterIntroduction,
        RewardType.NAVIGATIONBOOSTER => NavigationBoosterIntroduction,
        _ => default
    };

    public Sprite Icon(RewardType type) => GameManager.Instance.AssetScriptableData.GetSprite(type);

    public int LevelRequirenment(RewardType type) => type switch
    {
        RewardType.SIZEUPBOOSTER => SizeUpBoosterLvlRequirement-1,
        RewardType.FREEZETIMEBOOSTER => FreezeTimeBoosterLvlRequirement-1,
        RewardType.NAVIGATIONBOOSTER => NavigationBoosterLvlRequirement-1,
        _ => default

    };
}
[Serializable]
public class AdsSettings
{
    public int AdsMultiplierLevelReq;
    [SerializeField] bool m_useBanner;
    [SerializeField] float m_bannerSizeY;
    public bool UseBanner =>m_useBanner;
    public float BannerSizeY =>m_bannerSizeY;

    public void SetData(bool useBanner, float heightOffet) { 
        m_useBanner = useBanner;
        m_bannerSizeY = heightOffet;
    }
    

}


public enum Direction { 
    LEFT,RIGHT,TOP, BOTTOM,DIAGONAL
}
[Serializable]
public enum ClickableStates
{
    HIDDEN,
    PARTIAL,
    FULLY
}


[Serializable]
public struct SpinnerSettings
{
    [FoldoutGroup("Spinner rotation")]
    public float rotationLerpSpeed;
    [FoldoutGroup("Spinner rotation")]
    public float minRotationSpeed;
    [FoldoutGroup("Spinner rotation")]
    public float maxRotationSpeed;
    [FoldoutGroup("Spinner rotation")]
    public float rotationMultiplier;
   
    [FoldoutGroup("Spinner rotation")]
    public float hitBack;
    [FoldoutGroup("Spinner rotation"),GUIColor("yellow")]

    public float hitBackOnReleased;
    [FoldoutGroup("Spinner rotation"), GUIColor("yellow")]
    public float frictionOnReleased;
    [FoldoutGroup("Spinner rotation"), GUIColor("green")]
    public float notStrongEnoughKnockbackMultiplier;
}


public enum UnitVisualState
{
    NORMAL,
    SELECTED,
    SUGGESTEDMERGE,
    GRAYOUT
}
public enum CellHighlightType
{
    BUFF,
    AFK
}
public enum TwoStateChoice{ 
    Yes,No
}

public enum RewardType { 
    COIN,
    DIAMOND,
    SCORE,
    SIZEUPBOOSTER,
    FREEZETIMEBOOSTER,
    NAVIGATIONBOOSTER,
    TIMER,
    CHIP

}

[Serializable]
public struct RewardTypeData
{
    public RewardType RewardType;
    public int Amount;
    public Sprite Icon => GameManager.Instance.AssetScriptableData.GetSprite(RewardType);
}





#region Editor Creations

public enum ColorType
{
    BLUE, CYAN, GREEN, ORANGE, PINK, PURPLE, RED, YELLOW,WHITE,BLACK,GRAY
}

#endregion