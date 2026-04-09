using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class MessageSettings : PopUp
{

    [SerializeField] Button m_CloseButton;
    [SerializeField] Button m_RestartButton;
    [SerializeField] Button m_ContinueButton;


    [SerializeField] Button m_OverlayButton;
    [SerializeField] CanvasGroup OverlayGroup;
    [SerializeField] CanvasGroup m_ContentGroup;

    public List<SettingsCell> SettingItems = new List<SettingsCell>();
    [SerializeField] TMP_Text VersionText;

    private void Awake()
    {
        SettingItems = this.transform.GetComponentsInChildren<SettingsCell>().ToList();
        m_OverlayButton.onClick.AddListener(OnClose);
    }
    public void SetData()
    {
        m_RestartButton.onClick.AddListener(OnRestart);
        ButtonsSetup();
        ShowAnimation();
        VersionText.text = "Version " + Application.version;
    }

    private void ShowAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("OpenWindow");
        });
        s.Append(OverlayGroup.DOFade(1, 0.3f).From(0));
        s.Join(m_ContentGroup.transform.RectTransform().DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.4f, vibrato: 8).SetEase(Ease.OutElastic));
        s.AppendInterval(.2f);
        s.OnComplete(() => {
            Time.timeScale = 0;

        });
    }
    void ButtonsSetup()
    {
        m_CloseButton.onClick.RemoveAllListeners();
        m_CloseButton.onClick.AddListener(OnClose);

        m_ContinueButton.onClick.RemoveAllListeners();
        m_ContinueButton.onClick.AddListener(OnClose);

        var soundCell = SettingItems.Where(x => x.type == SettingsCell.SettingsType.SOUND).First();
        var musicCell = SettingItems.Where(x => x.type == SettingsCell.SettingsType.MUSIC).First();
        var vibrationCell = SettingItems.Where(x => x.type == SettingsCell.SettingsType.VIBRATION).First();



        OnValueChange(soundCell, GameManager.Instance.SettingsManager.SoundVolume);
        soundCell.toggle.onValueChanged.AddListener((value) => OnValueChange(soundCell, value));


        OnValueChange(musicCell, GameManager.Instance.SettingsManager.MusicVolume);
        musicCell.toggle.onValueChanged.AddListener((value) => OnValueChange(musicCell, value));

        OnValueChange(vibrationCell, GameManager.Instance.SettingsManager.Vibrations);
        vibrationCell.toggle.onValueChanged.AddListener((value) => OnValueChange(vibrationCell, value));


    }

    private void OnValueChange(SettingsCell cell, bool value)
    {

        cell.BG_Icon.color = value ? cell.BG_On : cell.BG_Off;
        cell.Handle.rectTransform.pivot = new Vector2(!value ? 1 : 0, 0.5f);

        switch (cell.type)
        {
            case SettingsCell.SettingsType.SOUND:
                cell.title.text = "Sound";
                GameManager.Instance.SettingsManager.SoundVolume = value;
                break;
            case SettingsCell.SettingsType.MUSIC:
                cell.title.text = "Music";
                GameManager.Instance.SettingsManager.MusicVolume = value;
                break;
            case SettingsCell.SettingsType.VIBRATION:
                cell.title.text = "Vibration";
                GameManager.Instance.SettingsManager.Vibrations = value;
                if (value) GameManager.Instance.HapticManager.VibradePreset( HapticManager.HapticType.PEEK);

                break;
        }
    }

    private void OnRestart() { 
        //GameManager.Instance.UIManager.RestartGame(force: true);
         OnClose();
    }
    private void OnClose()
    {
        Time.timeScale = 1;
        OnCompleteBase?.Invoke();

    }




}