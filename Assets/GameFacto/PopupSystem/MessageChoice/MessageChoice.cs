
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageChoice : PopUp
{
    [SerializeField] CanvasGroup m_ContentGroup;

    public TMP_Text TitleText;
    public TMP_Text MiddleText;
    [SerializeField] bool m_UseToggle;
    [ShowIf("m_UseToggle")]
    public Toggle Toggle;


    [Space(20)]
    [SerializeField, BoxGroup("Button")] Button m_YES_button;
    [SerializeField, BoxGroup("Button")] TMP_Text m_YES_buttonText;


    [Space(20)]
    [SerializeField, BoxGroup("Button")] Button m_NO_button;
    [SerializeField, BoxGroup("Button")] TMP_Text m_NO_buttonText;



    UnityAction<ChoiceResult> m_OnCompleteAction;
    // Update is called once per frame

    private void Awake()
    {
        if(!m_UseToggle && Toggle != null) Toggle.gameObject.SetActive(false);
        if (Toggle != null) Toggle.isOn = false;
    }

    public void SetData(ChoiceParameters data, UnityAction<ChoiceResult> OnComplete = null)
    {
       // m_TimerTitle.text = GameManager.Instance.LevelManager.LevelData.TimeLeft;
        m_OnCompleteAction = OnComplete;
        SetUpVisuals(data);
        ShowAnimation();
    }


    void OnYES_pressed()
    {
        OnClose(TwoStateChoice.Yes);
    }

    void OnNO_pressed()
    {
        OnClose(TwoStateChoice.No);
    }


    private void OnClose(TwoStateChoice choice)
    {
        m_ContentGroup.interactable = false;

        var responce = new ChoiceResult
        {
            choice = choice,
            isToggleChecked = Toggle.isOn

        };

        SetInteraction(interactable: false);
        m_OnCompleteAction?.Invoke(responce);

        HideAnimation().OnComplete(() => {
            OnCompleteBase?.Invoke();
            m_OnCompleteAction = null;
        });
        

    }

    void SetUpVisuals(ChoiceParameters data)
    {
        TitleText.text = data.TitleText.IsNullOrEmpty()?TitleText.text:data.TitleText;
        MiddleText.text =data.MiddleText.IsNullOrEmpty()?MiddleText.text:data.MiddleText;

        m_NO_buttonText.text = data.NoButtonText.IsNullOrEmpty()?m_NO_buttonText.text:data.NoButtonText;
        m_YES_buttonText.text = data.YesButtongText.IsNullOrEmpty()?m_YES_buttonText.text:data.YesButtongText;


        m_YES_button.onClick.AddListener(OnYES_pressed);
        m_NO_button.onClick.AddListener(OnNO_pressed);

    }



    private void ShowAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
           SetInteraction(interactable: false);
           GameManager.Instance.SoundManager.PlayGivenSound("Pop",volume: 0.2f);
        });
;
        s.Append(m_ContentGroup.DOFade(1, 0.2f).From(0));
        s.Join(m_ContentGroup.transform.RectTransform().DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, vibrato: 8).SetEase(Ease.OutElastic));

        s.AppendCallback(() => { SetInteraction(interactable: true); });
    }
    private Tween HideAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("Sweesh", volume: 0.1f);
        });
        ;
        s.Append(m_ContentGroup.DOFade(0, 0.1f));
        s.Join(m_ContentGroup.transform.RectTransform().DOScale(0.8f,0.1f).SetEase(Ease.InBack));

        return s;
    }

    public void SetInteraction(bool interactable)
    {
        m_YES_button.interactable = interactable;
        m_NO_button.interactable = interactable;
    }


    public struct  ChoiceResult {

        public TwoStateChoice choice;
        public bool isToggleChecked;

    }

    public struct ChoiceParameters
    {
        public string TitleText;
        public string MiddleText;

        public string YesButtongText;
        public string NoButtonText;

    }

  
}
