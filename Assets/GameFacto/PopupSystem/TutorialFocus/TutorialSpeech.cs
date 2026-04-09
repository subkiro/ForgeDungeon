using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class TutorialSpeech : MonoBehaviour
{
    public Image AvatarImage;
    public TMP_Text Speechtext;
    public CanvasGroup MainGroup, SpeechContainerGroup,AvatarGroup,BackgroundGroup;
    private RectTransform SpeechContainerRect, AvatarRect;
    private float m_avatarMovePivotDirection;
    public Sprite[] AvatarSprites;


    [Space]
    public VerticalLayoutGroup vGroup;
    public HorizontalLayoutGroup HGroup;

    public LayoutElement SpeechContainerLayout;


    private MessageTeachingFocus.TutorialSpeechData m_speechData;


    private void Awake()
    {
        SpeechContainerRect = SpeechContainerGroup.GetComponent<RectTransform>();
        AvatarRect = AvatarGroup.GetComponent<RectTransform>();    

        AvatarGroup.alpha = 0;
        SpeechContainerGroup.alpha = 0;
        MainGroup.alpha = 0;
    }
    public void SetData(MessageTeachingFocus.TutorialSpeechData speechData)
    {

        this.vGroup.transform.RectTransform().anchorMin = speechData.AnchorMin;
        this.vGroup.transform.RectTransform().anchorMax = speechData.AnchorMax;

        SpeechContainerLayout.preferredWidth = speechData.Prefered_Width;
        m_speechData = speechData;

        SetUpSpeechDataPositions(m_speechData);
        AvatarImage.sprite = AvatarSprites[(m_speechData.spriteIdex >= AvatarSprites.Length) ? 0 : m_speechData.spriteIdex];
        Animate();

    }


    private void SetUpSpeechDataPositions(MessageTeachingFocus.TutorialSpeechData speechData)
    {
        switch (speechData.childAligment)
        {
           
            case TextAnchor.LowerLeft:
                m_avatarMovePivotDirection = 1;
                vGroup.reverseArrangement = true;
                HGroup.reverseArrangement = false;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(1f, 1f);

                break;
            case TextAnchor.LowerRight:
                m_avatarMovePivotDirection = 0;
                vGroup.reverseArrangement = true;
                HGroup.reverseArrangement = true;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(-1f, 1f);


                break;
            case TextAnchor.UpperLeft:
                m_avatarMovePivotDirection = 1;
                vGroup.reverseArrangement = false;
                HGroup.reverseArrangement = false;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(1f, 1f);

                break;
            case TextAnchor.UpperRight:
                m_avatarMovePivotDirection = 0;
                vGroup.reverseArrangement = false;
                HGroup.reverseArrangement = true;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(-1f, 1f);

                break;
            case TextAnchor.LowerCenter:
                m_avatarMovePivotDirection = 1;
                vGroup.reverseArrangement = true;
                HGroup.reverseArrangement = true;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(1f, 1f);

                break;
            default:
                m_avatarMovePivotDirection = 1;
                vGroup.reverseArrangement = false;
                HGroup.reverseArrangement = false;
                SpeechContainerGroup.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
                BackgroundGroup.transform.RectTransform().localScale = new Vector2(1f, 1f);


                break;
        }
    }

    private void Animate()
    {
        Sequence s = DOTween.Sequence();
        s.SetLink(this.gameObject);
        s.SetUpdate(isIndependentUpdate: true);

        s.Append(MainGroup.DOFade(1, 0).From(0));

        
        s.Join(AvatarGroup.DOFade(1, 0.2f).From(0));
        s.Join(AvatarRect.DOPivotX(AvatarRect.pivot.x, 0.3f).From(new Vector2(m_avatarMovePivotDirection,0.5f)).SetEase(Ease.OutBack));
        s.Join(AvatarGroup.transform.DOScale(Vector3.one,0.3f).From(new Vector3(1.1f,0.3f,1)).SetEase(Ease.OutElastic));

        s.Append(SpeechContainerGroup.DOFade(1, 0.2f).From(0));
        RectTransform rect = SpeechContainerGroup.GetComponent<RectTransform>();
       // s.Join(rect.DOPivotX(HGroup.reverseArrangement ? 1:0, 0.2f).From(new Vector2(HGroup.reverseArrangement ? 0.7f : 0.3f, 0.5f)).SetRelative().SetDelay(0.1f));
        s.Join(rect.DOPivotX(rect.pivot.x, 0.2f).From(new Vector2(HGroup.reverseArrangement ? 0.7f : 0.3f, 0.5f)).SetRelative().SetDelay(0.1f));

        s.Join(Speechtext.DOText(m_speechData.Speechtext, 1f).From(""));
    }

 

   


   

}
