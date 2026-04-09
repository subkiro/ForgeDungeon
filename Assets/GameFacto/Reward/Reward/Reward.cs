using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Threading.Tasks;

public class Reward : SingletonObj<Reward>
{
    [SerializeField] private Image RewardItemUI;
   


    public int maxCount = 2;
    [Range(0.1f, 0.3f)] public float minDuration;
    [Range(0.3f, .8f)] public float maxDuration;
    public Ease easeType = Ease.OutSine;

    public string TweenID = "Reward";
    public Vector2 defaultDeltaSize = Vector2.one*150;
    Queue<Image> reward_UI_Items_Queue = new Queue<Image>();

   
    private void Start()
    {
        Prepare(maxCount);
    }

    
    public Sequence  AnimateSpread(RewardType type, Vector3 startPos, Transform endPos, int amount, UnityAction OnComplete, float StartScaleMultiplier = 1, string TweenID = "Reward",  bool previewAnimation = true,float previewInterval = 0.5f, float animDurationMult = 1, float spreadDistance = 3, float IntervalBeforeComplete = 0,Sprite icon = null)
    {
        //Vector2 CenterPosOfScreen = new Vector2( Screen.width/2, Screen.height/2);
        amount = Mathf.Clamp(amount, 0, maxCount);
        Vector2 CenterPosOfScreen = Vector2.zero;
        Tools.Log("endPos.position" + endPos.position);

        
        var destinationPos = endPos.position;

        if (startPos == Vector3.zero) startPos = CenterPosOfScreen; 
        this.TweenID = TweenID;
        float StartDelay = .1f;

        Sequence mainSeq = DOTween.Sequence();
        mainSeq.SetId(this.TweenID);


        for (int i = 0; i < amount; i++)
        {
          

            if (reward_UI_Items_Queue.Count > 0)
            {
                Sequence s = DOTween.Sequence();
                s.SetUpdate(true);
                s.AppendInterval(StartDelay);
                Image rewardObj = reward_UI_Items_Queue.Dequeue();
                //put the image inside
                rewardObj = Setup(rewardObj, type, icon);
                rewardObj.gameObject.SetActive(true);
                rewardObj.transform.position = startPos;
                rewardObj.transform.localPosition = startPos;

                var endPosRect = endPos.GetComponent<RectTransform>();
                Vector2 initialSize = new Vector2(endPosRect.rect.width, endPosRect.rect.height);


                //animate coin
                float duration = Random.Range(minDuration, maxDuration) * animDurationMult;
                
                var rewardRect = rewardObj.rectTransform;
                var defaultSize = rewardRect.sizeDelta;

                float rangeDistance = Screen.width > Screen.height ? Screen.height : Screen.width;
                //Part1 ShowReward
                
                Vector3 offset =spreadDistance>0? new Vector3(Random.Range(-rangeDistance / spreadDistance, rangeDistance / spreadDistance), Random.Range(-rangeDistance / spreadDistance, rangeDistance / spreadDistance), 0) : Vector3.zero;

                Vector3 randomStartPos = startPos + offset;


                s.Append(rewardRect.DOScale(Vector3.one*StartScaleMultiplier, duration* animDurationMult).From(Vector2.zero).SetEase(Ease.OutBack));
                s.Join(rewardObj.transform.RectTransform().DOAnchorPos(randomStartPos, duration* animDurationMult));
                if (previewAnimation) s.Append(rewardObj.transform.DOPunchScale(Vector3.one* StartScaleMultiplier * .3f, 0.2f, 0, duration* animDurationMult).OnStart(() => {
                    GameManager.Instance.SoundManager.PlayGivenSound("Coin", volume: 0.1f);
                }));
                //s.Join(coin.transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetDelay(0.1f));
                s.AppendInterval(previewInterval);
                s.Append(rewardRect.DOSizeDelta(initialSize, duration).SetEase(Ease.OutQuart));
                s.Join(rewardRect.DOScale(Vector3.one, duration));

                s.Join(rewardObj.transform.DOMove(destinationPos, duration).SetEase(easeType).OnComplete(() =>
                {
                    //GameManager.Instance.SoundManager.PlayGivenSound(CoinSound, SoundManager.AudioMixerGroups.Sfx, 1);
                   if(!DOTween.IsTweening(endPos)) endPos.DOPunchScale(Vector3.one * 0.2f, 0.1f, 1, 1);
                    PlaySound(type);
                    rewardObj.gameObject.SetActive(false);
                    rewardRect.sizeDelta = defaultSize;
                    reward_UI_Items_Queue.Enqueue(rewardObj);

                }));

                mainSeq.Join(s);
            }
        }

        mainSeq.OnStart(() => { });
        mainSeq.AppendInterval(IntervalBeforeComplete);
        mainSeq.OnComplete(() => OnComplete?.Invoke());

        return mainSeq;

    }



    public void Prepare(int maxAmount)
    {

        Image g;
        for (int i = 0; i < maxAmount; i++)
        {

            g = Instantiate(RewardItemUI, this.transform);
            g.gameObject.SetActive(false);
            reward_UI_Items_Queue.Enqueue(g);
        }

    }


  
    public Image Setup(Image im, RewardType type,Sprite icon)
    {
        im.sprite = icon!=null? icon: GameManager.Instance.AssetScriptableData.GetSprite(type);    
        return im;
    }


  

    private void PlaySound(RewardType type)
    {
        switch (type)
        {
            case RewardType.COIN:
                GameManager.Instance.SoundManager.PlayGivenSound("Coin", volume: 0.1f, pitch: 1+Random.value);
                break;
        }
    }

    public void OnDestroy()
    {
        DOTween.Kill(TweenID,true);
    }



    public List<Image> GetPoolItems(int amout) {
        var list = new List<Image>();

        for (int i = 0; i < amout; i++)
        {
            if (reward_UI_Items_Queue.Count == 0) { 
                break;
            }
            var item = reward_UI_Items_Queue.Dequeue();
            item.gameObject.SetActive(true);

            list.Add(item);
        }

        return list;
    }

    public void ReleasePoolItems(List<Image> items) {
        foreach (var item in items)
        {
            item.gameObject.SetActive(false);
            item.rectTransform.sizeDelta = defaultDeltaSize;
            item.transform.localEulerAngles= Vector3.zero;

            reward_UI_Items_Queue.Enqueue(item);
        }
    }


}
