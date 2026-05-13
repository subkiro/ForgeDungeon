using Coffee.UIEffects;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class UIManager : MonoBehaviour
{

    [SerializeField] CanvasGroup m_HUDGroup;

    [SerializeField] Button m_SettingsButtonInGame;
    [SerializeField] Button m_ChoiceButton;
    [SerializeField] Button m_PlayerProfileButton;
    [SerializeField] Button m_RPSButton;
    public Button SettingButton => m_SettingsButtonInGame;


    Canvas m_UICanvas;








    internal void Initialize()
    {
        m_UICanvas = this.GetComponent<Canvas>();
        m_UICanvas.worldCamera = GameManager.Instance.CameraManager.MainCamera;
        m_SettingsButtonInGame.onClick.AddListener(ShowSettings);
        m_ChoiceButton.onClick.AddListener(DebugChoiceButton);
        m_PlayerProfileButton.onClick.AddListener(DebugPlayerProfile);
        m_RPSButton.onClick.AddListener(ShowRPS);

    }

    private void DebugPlayerProfile()
    {
        ShowPlayerProfile();
    }

    private void DebugChoiceButton()
    {
        ShowChoice(default, OnChoiceMade);
        void OnChoiceMade(MessageChoice.ChoiceResult choice)
        {
            Tools.Log(choice.choice.ToString());
        }
    }

    private void ShowSettings()
    {
        var message = PopUpManager.Instance.ShowSimple<MessageSettings>(GameManager.Instance.AssetScriptableData.MessageSettings, FadeOutSpeed: 0.01f);
        message.SetData();
    }

    public void ShowChoice(MessageChoice.ChoiceParameters parametes, UnityAction<MessageChoice.ChoiceResult> result)
    {
        var message = PopUpManager.Instance.ShowSimple<MessageChoice>(GameManager.Instance.AssetScriptableData.MessageChoice, FadeOutSpeed: 0.01f);
        message.SetData(parametes, result);


    }

    public void ShowPlayerProfile()
    {
        var message = PopUpManager.Instance.ShowSimple<MessagePlayerProfile>(GameManager.Instance.AssetScriptableData.MessagePlayerProfile, FadeOutSpeed: 0.01f);
        message.SetData();


    }
    public void ShowRPS()
    {
        var message = PopUpManager.Instance.ShowSimple<MessageRPS>(GameManager.Instance.AssetScriptableData.MessageRPS, FadeOutSpeed: 0.01f);
        message.SetData();


    }

    [Button]
    void GiveRewardsDebug()
    {
        if (GameManager.Instance.Player.Coins == null)
        {
            Debug.LogError("Coins is NULL");
            return;
        }
        GameManager.Instance.Player.Coins.Value += 11;

        return;
        Reward.Instance.AnimateSpread(RewardType.COIN, Vector2.zero, StatManager.Instance.CoinStatCell.StatIcon.transform, 10, () =>
                {
                    GameManager.Instance.Player.Coins.Value += 77;
                });
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
