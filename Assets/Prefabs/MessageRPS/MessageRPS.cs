using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageRPS : PopUp
{
    [SerializeField] Button m_CloseButton;
    [SerializeField] CanvasGroup m_ContentGroup;
    [SerializeField] RectTransform CenterDeckRect_player;
    [SerializeField] RectTransform CenterDeckRect_opponet;

    [SerializeField] List<CardsData> PlayerCardsData;
    [SerializeField] List<CardsData> OpponentCardsData;


    private CardsData m_active_PlayerCard;
    private CardsData m_active_OpponentCard;

    [SerializeField] TMP_Text m_ResultText;
    [SerializeField] TMP_Text m_PlayerHelpText;

    private int m_RoundCount;
    private int m_winCount;
    private int m_lostCount;
    private int m_drawCount;

    private OnAnyTapAction m_Tap;
    public void SetData()
    {
        // StatManager.Instance.Show(true,instant: true,front: true);
        SetupCards();
        ShowAnimation();
        m_Tap = this.gameObject.AddComponent<OnAnyTapAction>();
        m_Tap.SetOnClose(ShowDebug);

        ShowBottomMessage(show: true, "Tap to Start");

        m_CloseButton.onClick.AddListener(() => _ = OnClose());
    }
    [Button]
    async void ShowDebug()
    {
        ShowBottomMessage(show: false);

        ShowCards(PlayerCardsData, isPlayer: true).Forget();
        ShowCards(OpponentCardsData, isPlayer: false).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(1));

        var (success, card) = await WaitToSelectCard();

        await SelectCard(card, isPlayer: true);
        await SelectCard(GetRandomCard(OpponentCardsData), isPlayer: false);

        var results = await ShowResult();

        HideCards(PlayerCardsData, isPlayer: true).Forget();
        HideCards(OpponentCardsData, isPlayer: false).Forget();


        if(m_winCount==2 || m_lostCount == 2)
        {
            await ShowFinalResults(results);
            ShowBottomMessage(show: true, "Tap to Return");
            m_Tap.SetOnClose(()=>{
                _=OnClose();
                if(results== RPS_Result.win)
                {
                   GiveRewards();
                }
                });
            return;
        }

        await ShowRound();

        ShowBottomMessage(show: true, "Tap to Continue");
        m_Tap.SetOnClose(ShowDebug);
    }


    void SetupCards()
    {
        foreach (var item in PlayerCardsData)
        {
            var button = item.card.Container.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => SelectCardAction(item));
            item.card.Interactable = false;
        }

    }

    private void SelectCardAction(CardsData cardData)
    {
        Tools.Log($"{cardData.card.gameObject.name}");
        _tcs.TrySetResult((true, cardData));
    }

    private UniTaskCompletionSource<(bool, CardsData)> _tcs;

    UniTask<(bool, CardsData)> WaitToSelectCard()
    {
        _tcs = new UniTaskCompletionSource<(bool, CardsData)>();
        return _tcs.Task;
    }









    //Animations Process

    // 1
    async UniTask ShowCards(List<CardsData> dataList, bool isPlayer)
    {
        var tasks = new List<UniTask>();
        float duration = .5f;
        if (!isPlayer) ShuffleOpponentCards();

        foreach (var itemData in dataList)
        {

            var item = itemData.card;
            var targetDest = (CenterDeckRect_opponet.localPosition + CenterDeckRect_player.localPosition) / 2;
            item.ResetCard();
            item.IsHidden = true;
            tasks.Add(item.Group.DOFade(1, duration / 2).From(0).SetLink(this.gameObject).ToUniTask());


        }
        await UniTask.WhenAll(tasks);
        tasks.Clear();

        GameManager.Instance.SoundManager.PlayGivenSound("CardSelect",volume: 0.5f);

        foreach (var itemData in dataList)
        {
            var item = itemData.card;
            var targetDest = itemData.CardsPositions;
            item.IsHidden = !itemData.isPlayer;
            float delay = UnityEngine.Random.Range(0, .1f);
            tasks.Add(item.transform
            
            .DOLocalMove(targetDest.localPosition, duration)     
            .SetDelay(delay)
            .SetEase(Ease.OutBack)
            .SetLink(this.gameObject)
            .ToUniTask());
            

        }

        await UniTask.WhenAll(tasks);

        foreach (var itemData in dataList)
        {
            itemData.card.Interactable = isPlayer;
        }

        if (isPlayer)
        {
            ShowBottomMessage(show: true, "Select Card");

        }
    }

    // 2
    async UniTask SelectCard(CardsData SelectedCard, bool isPlayer)
    {
        m_PlayerHelpText.DOKill();
        m_PlayerHelpText.alpha = 0;

        var card = SelectedCard.card;
        if (isPlayer) m_active_PlayerCard = SelectedCard;
        else m_active_OpponentCard = SelectedCard;


        var deck = isPlayer ? CenterDeckRect_player : CenterDeckRect_opponet;
        GameManager.Instance.SoundManager.PlayGivenSound(isPlayer? "CardSelect":"CardDisselect");


        var tasks = new List<UniTask>();
        float duration = .3f;


        tasks.Add(card.transform.RectTransform().DOJumpAnchorPos(
            deck.anchoredPosition,
            500,
            1,
            duration
        ).SetLink(this.gameObject).SetEase(Ease.OutQuad).ToUniTask());

        var restCards = isPlayer ? PlayerCardsData : OpponentCardsData;
        foreach (var item in restCards)
        {
            item.card.Interactable = false;
            if (item.card == SelectedCard.card) continue;
            tasks.Add(item.card.Group.DOFade(0.2f, duration).SetLink(this.gameObject).ToUniTask());
            tasks.Add(item.card.transform.RectTransform().DOScale(0.8f, duration).SetLink(this.gameObject).ToUniTask());
        }



        tasks.Add(card.transform.DOLocalRotate(Vector3.zero, duration).SetLink(this.gameObject).ToUniTask());

        await UniTask.WhenAll(tasks);
        tasks.Clear();
        
        
        if (!SelectedCard.isPlayer) await FlipCard(SelectedCard, duration / 4).SetLink(this.gameObject).ToUniTask();

    }

    // 3
    async UniTask<RPS_Result> ShowResult()
    {
        var result = GetResult(m_active_PlayerCard.card.ID, m_active_OpponentCard.card.ID);
        string message = result.ToString();


        var tasks = new List<UniTask>();
        CardRPS_Cell wincell;
        Vector2 hitDestination = result == RPS_Result.win ? m_active_OpponentCard.card.transform.position : m_active_PlayerCard.card.transform.position;

        switch (result)
        {
            case RPS_Result.win:
                m_winCount++;
                m_active_PlayerCard.card.transform.SetAsLastSibling();
                wincell = m_active_PlayerCard.card;
               GameManager.Instance.SoundManager.PlayGivenSound("PositiveFeedback",volume:.5f);

                await Hit(wincell.transform.RectTransform(), hitDestination);
                break;

            case RPS_Result.lost:
                m_lostCount++;
                m_active_OpponentCard.card.transform.SetAsLastSibling();
                wincell = m_active_OpponentCard.card;
                GameManager.Instance.SoundManager.PlayGivenSound("NegativeFeedback",volume:.5f);

                await Hit(wincell.transform.RectTransform(), hitDestination);
                break;

            case RPS_Result.draw:
                GameManager.Instance.SoundManager.PlayGivenSound("CardResult",volume:.5f);

                await HitDraw(m_active_PlayerCard.card.transform.RectTransform(),m_active_OpponentCard.card.transform.RectTransform());
                m_drawCount++;

                break;

        }




        await ShowMessage(message, 1);



       Tween Hit(RectTransform winCardRect, Vector2 dest)
        {
            return winCardRect.DOJumpAnchorPos(
             dest,
             300,
             1,
             .2f
         ).SetLink(winCardRect.gameObject).SetEase(Ease.OutQuad);
        }

        Tween HitDraw(RectTransform cardA,RectTransform cardB)
        {

            var s  = DOTween.Sequence();

            s.Join(cardA.DOPunchAnchorPos(Vector2.right*50f,.2f));
            s.Join(cardB.DOPunchAnchorPos(Vector2.left*50f,.2f));

            s.Join(cardA.DOPunchRotation(Vector3.forward*-20,.2f).SetEase(Ease.InBack));
            s.Join(cardB.DOPunchRotation(Vector3.forward*20,.2f).SetEase(Ease.InBack));

            return s;

            
        }


        return result;
    }
    // 4
     async UniTask HideCards(List<CardsData> dataList, bool isPlayer)
    {
        var activeData = isPlayer ? m_active_PlayerCard : m_active_OpponentCard;






        var tasks = new List<UniTask>();
        float duration = .3f;
        float finalPos = 800;
        float dir = isPlayer ? -finalPos : finalPos;

        foreach (var item in dataList)
        {

            tasks.Add(item.card.Group.DOFade(0, duration / 2).SetDelay(.1f).ToUniTask());

        }

        foreach (var item in dataList)
        {
            if (activeData.card == item.card) continue;

            float delay = UnityEngine.Random.Range(0, .1f);
            float dest = item.CardsPositions.localPosition.y + dir;
            tasks.Add(item.card.transform
            .DOLocalMoveY(dest, duration)
            .SetDelay(delay)
            .SetEase(Ease.InFlash)
            .ToUniTask());

        }

        tasks.Add(activeData.card.transform
            .DOLocalMoveX(dir, duration)
            .SetEase(Ease.InBack)
            .ToUniTask());

        await UniTask.WhenAll(tasks);


        if (isPlayer) m_active_PlayerCard = default;
        if (!isPlayer) m_active_OpponentCard = default;   

    }

    //5 
    async UniTask ShowRound()
    {
        m_RoundCount++;
        string message = $"Round {m_RoundCount} Score {m_winCount} - {m_lostCount}";
        await ShowMessage(message, 1);
    }

     UniTask ShowFinalResults(RPS_Result result)
    {

        string message = $"Score {m_winCount} - {m_lostCount}";
        string res = "";
        switch (result)
        {
            case RPS_Result.win:
            res = "\n<color=green>You win";
            break;
            case RPS_Result.lost:
            res = "\n<color=red>You Lost";
            break;
        }

        
        
        return ShowMessage(message+res, 1);
    }






    #region  Help Functions


    void GiveRewards()
    {
        
            Reward.Instance.AnimateSpread(RewardType.COIN,Vector2.zero, StatManager.Instance.CoinStatCell.StatIcon.transform, 10, () =>
                    {
                        GameManager.Instance.Player.Coins.Value+=10;
                    });
    }
    void ShowBottomMessage(bool show, string message = "", int loops = -1)
    {
        m_PlayerHelpText.DOKill();
        if (show)
        {
            m_PlayerHelpText.text = message;
            m_PlayerHelpText.DOFade(1, .5f).From(0).SetLoops(-1, LoopType.Yoyo).SetLink(this.gameObject);
        }
        else
        {
            m_PlayerHelpText.alpha = 0;
            m_PlayerHelpText.text = "";

        }
    }
    UniTask ShowMessage(string message, float waitTime)
    {

        m_ResultText.text = message;
        m_ResultText.rectTransform.localScale = Vector2.one;
        m_ResultText.alpha = 0;

        var s = DOTween.Sequence()
        .Append(m_ResultText.DOFade(1, .3f))
        .Append(m_ResultText.rectTransform.DOPunchScale(Vector2.one * .1f, .2f))
        .AppendInterval(1)
        .Append(m_ResultText.DOFade(0, .3f))
        .ToUniTask();
        return s;
    }
    void ShuffleOpponentCards()
    {
        for (int i = 0; i < OpponentCardsData.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, OpponentCardsData.Count);

            // swap
            (OpponentCardsData[i].card, OpponentCardsData[randomIndex].card) =
                (OpponentCardsData[randomIndex].card, OpponentCardsData[i].card);
        }
    }

    CardsData GetRandomCard(List<CardsData> cards)
    {
        var card = cards[UnityEngine.Random.Range(0, cards.Count)];
        return card;
    }
    Tween FlipCard(CardsData cardData, float duration)
    {
        var flipTween = cardData.card.transform.DOScale(new Vector3(0, 1, 1), duration)
        .From(Vector3.one)
        .SetLoops(2, LoopType.Yoyo)
        .OnStepComplete(() =>
        {
            cardData.card.IsHidden = false;
        });
        return flipTween;
    }
    RPS_Result GetResult(RPS_ID player, RPS_ID opponent)
    {
        if (player == opponent)
            return RPS_Result.draw;

        return (player, opponent) switch
        {
            (RPS_ID.Rock, RPS_ID.Scissor) => RPS_Result.win,
            (RPS_ID.Paper, RPS_ID.Rock) => RPS_Result.win,
            (RPS_ID.Scissor, RPS_ID.Paper) => RPS_Result.win,

            _ => RPS_Result.lost
        };
    }
    #endregion


    #region  Show/Hide Animations
    private void ShowAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.SetLink(this.gameObject);
        s.OnStart(() =>
        {
            GameManager.Instance.SoundManager.PlayGivenSound("Pop", volume: 0.2f);
        });
        ;
        s.Append(m_ContentGroup.DOFade(1, 0.2f).From(0));
        s.Join(m_ContentGroup.transform.RectTransform().DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, vibrato: 8).SetEase(Ease.OutElastic));


    }
    private async Awaitable OnClose()
    {
        m_ContentGroup.interactable = false;
        await HideAnimation().ToAwaitable();
        // _=StatManager.Instance.Show(show: false,instant: true,front: true);

        OnCompleteBase?.Invoke();

    }

    private Tween HideAnimation()
    {

        //Show Animation
        Sequence s = DOTween.Sequence();
        s.SetId(this);
        s.SetLink(this.gameObject);
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


[Serializable]
public class CardsData
{
    public bool isPlayer;
    public CardRPS_Cell card;
    public RectTransform CardsPositions;

}

public enum RPS_Result
{
    win,
    lost,
    draw
}


