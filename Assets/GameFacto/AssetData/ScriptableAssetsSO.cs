using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Create Assets SO")]


public class ScriptableAssetsSO : ScriptableObject
{
   
    public List<TutorialFeatureSO> TutorialFeatures;
    [TabGroup("LevelData")]
    public List<int> ProgressLevelValues;


   [TabGroup("PopUp")]
    public GameObject MessageSettings;
    [TabGroup("PopUp")]
    public GameObject MessageForgeRoom;




    [TabGroup("Prefabs")]
    public RewardUiItem RewardUIItem;



    public Sprite Coin_Icon;
    [TabGroup("Sprites")]
    public Sprite Default_EmptyIcon;
    [TabGroup("LevelData")]


    
    public Sprite GetSprite(RewardType type)
    {
        switch (type)
        {
            case RewardType.COIN :
                return Coin_Icon;


            default:
                return Default_EmptyIcon;
        }
    }

    #region Tutorial Features
    public TutorialFeatureSO ReturnTutorialFeatureData(int index, bool isTutorial, bool exactLevel)
    {
        List<TutorialFeatureSO> data;
        int levelReq = index;


        int maxLevel = levelReq;


        data = TutorialFeatures.Where(w => w.IsTutorial == isTutorial).ToList();

        if (data.Count > 0)
        {
            maxLevel = data.Max(x => x.MaxLevelRequirement - 1);
            levelReq = levelReq > maxLevel ? maxLevel : levelReq;
        }

        return exactLevel ? data.First(w => levelReq == w.MaxLevelRequirement - 1) : data.First(w => levelReq > w.MinLevelRequirement && levelReq <= w.MaxLevelRequirement - 1);
    }

    public bool HasFeatureData(int levelReq, int stage = 0)
    {
        var allFeatures = TutorialFeatures.Where(w => w.IsTutorial == false);

        if (allFeatures == null || allFeatures.Count() == 0) return false;

        int maxLevel = allFeatures.Max(x => x.MaxLevelRequirement - 1);

        return allFeatures.Where(w => w.MaxLevelRequirement - 1 == levelReq && levelReq <= maxLevel).Any();
    }

    public bool HasTutoriaData(int levelReq)
    {
        var allTutorials = TutorialFeatures.Where(w => w.IsTutorial == true);
        if (allTutorials == null || allTutorials.Count() == 0) return false;

        int maxLevel = allTutorials.Max(x => x.MaxLevelRequirement - 1);

        return allTutorials.Where(w => w.MaxLevelRequirement - 1 == levelReq && levelReq <= maxLevel).Any();
    }

    #endregion



#if UNITY_EDITOR
    public static ScriptableAssetsSO GetAssetsData()
    {
        string path = "Assets/GameFacto/AssetData/AssetData.asset";
        ScriptableAssetsSO AssetData = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableAssetsSO>(path);
        return AssetData;
    }
#endif

}
