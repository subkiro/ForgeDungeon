using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageItemInfo : PopUp
{
    


    [SerializeField] CanvasGroup m_ContentGroup;
    [SerializeField] ItemCellView m_PreviewCellView;
    [SerializeField] Button m_SellButton_Yes;
    [SerializeField] Button m_CloseButton_No;

    [SerializeField] TMP_Text m_PreviewDescription;
    private UnityAction<TwoStateChoice> m_onChoiceAction;
    public void SetData(InventoryItemSO item,UnityAction<TwoStateChoice> _OnClose)
    {
        m_onChoiceAction = _OnClose;
        m_PreviewCellView.Initialize(item);
        m_CloseButton_No.onClick.AddListener(()=>_=OnClose(TwoStateChoice.No));
        m_SellButton_Yes.onClick.AddListener(()=>_=OnClose(TwoStateChoice.Yes));

        _=ShowAnimation();
    }

   

    #region  Show/Hide Animations
    private async Awaitable ShowAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("Pop", volume: 0.2f);
        });
        ;
        s.Append(m_ContentGroup.DOFade(1, 0.2f).From(0));
        s.Join(m_ContentGroup.transform.RectTransform().DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, vibrato: 8).SetEase(Ease.OutElastic));

        await s.ToAwaitable();
    }
    private async Awaitable OnClose(TwoStateChoice choice)
    {
        m_ContentGroup.interactable = false;
        await HideAnimation().ToAwaitable();
        m_onChoiceAction?.Invoke(choice);
        OnCompleteBase?.Invoke();

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
        s.Join(m_ContentGroup.transform.RectTransform().DOScale(0.8f, 0.1f).SetEase(Ease.InBack));

        return s;
    }
    #endregion
}
