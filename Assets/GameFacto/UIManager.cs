using Coffee.UIEffects;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class UIManager : MonoBehaviour
{

    [SerializeField] CanvasGroup m_HUDGroup;

    [SerializeField] Button m_SettingsButtonInGame;
    public Button SettingButton=> m_SettingsButtonInGame;

    Canvas m_UICanvas;
   







    internal void Initialize()
    {
        m_UICanvas = this.GetComponent<Canvas>();
        m_UICanvas.worldCamera = GameManager.Instance.CameraManager.MainCamera;
        m_SettingsButtonInGame.onClick.AddListener(ShowSettings);
       

    }

    private void ShowSettings()
    {
        var message = PopUpManager.Instance.ShowSimple<MessageSettings>(GameManager.Instance.AssetScriptableData.MessageSettings, FadeOutSpeed: 0.01f);
        message.SetData();
    }





    // public void CheckForTutorials(ActiveLevelData activeLevel)
    // {
    //     int index = activeLevel.WaveIndex;

    //     if (GameManager.Instance.AssetManager.AssetScriptableData.HasFeatureData(index))
    //     {
    //         var data = GameManager.Instance.AssetManager.AssetScriptableData.ReturnTutorialFeatureData(index, isTutorial: false, exactLevel: true);
    //         GameManager.Instance.TutorialManager.CallTutorial(data);

    //     }
    //     else if (GameManager.Instance.AssetManager.AssetScriptableData.HasTutoriaData(index))
    //     {
    //         var data = GameManager.Instance.AssetManager.AssetScriptableData.ReturnTutorialFeatureData(index, isTutorial: true, exactLevel: true);
    //         GameManager.Instance.TutorialManager.CallTutorial(data);

    //     }
       

    // }


}
