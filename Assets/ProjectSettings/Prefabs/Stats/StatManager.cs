using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Sirenix.OdinInspector;
[RequireComponent(typeof(Canvas))]
public class StatManager : SingletonObj<StatManager>
{
    [SerializeField] RectTransform m_ContentRect;
    private StatCellView[] m_StatCells;
    private Canvas m_Canvas;
    public Canvas StatCanvas=>m_Canvas;
    public override void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_StatCells = GetComponentsInChildren<StatCellView>(true);
    }

    public Tween Show(bool show, bool instant = false, bool front = true) {

      //  m_Canvas.overrideSorting = front;
        m_Canvas.sortingOrder = front? 300:0;

       

        if (instant) { 
           // m_ContentRect.gameObject.SetActive(show);
            return DOVirtual.DelayedCall(0,()=>{ });
        }
        var tween = m_ContentRect.DOPivotX(show ? 0 : 1, .2f).SetEase(show ? Ease.OutBack : Ease.InBack);
        
        //if(show)  tween.OnStart(()=>m_ContentRect.gameObject.SetActive(true));
        //else tween.OnComplete(() => m_ContentRect.gameObject.SetActive(false));

        return tween;
    }
    [Button]
    public void ShakeCoinContainer()
    {

        Debug.Log("SHAKING SLOT");

        GameManager.Instance.SoundManager.PlayGivenSound("",volume: 0.05f);

        CoinStatCell.transform.DOKill(true);
        CoinStatCell.transform.DOPunchPosition(Vector3.left*20, 0.3f, vibrato: 20, elasticity: 1);
    }

    public StatCellView CoinStatCell=>m_StatCells.Where(x=>x.RewardType==RewardType.COIN).FirstOrDefault();


}
