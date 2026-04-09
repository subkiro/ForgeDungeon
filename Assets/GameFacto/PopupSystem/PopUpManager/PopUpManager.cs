using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;

public class PopUpManager : SingletonObj<PopUpManager>
{
    #region PreloadPrefabs

   
    private List<PopUp> poolListPrefab =new List<PopUp>();

    private T DequeFromPoolPrefabs<T>(GameObject prefab, Transform parentObject)
    {
        var type = typeof(T).ToString();
        var exists = poolListPrefab.Exists(x=>x is T);
   

        if (exists)
        {
            var popup = poolListPrefab.First(x => x is T);
            poolListPrefab.Remove(popup);

            Tools.Log($"Dequed From Pool {type}");
            popup.gameObject.SetActive(true);
            popup.transform.SetParent(parentObject, false);
            popup.transform.SetAsLastSibling();

            return popup.GetComponent<T>(); 
        }
        else {

            Tools.Log($"Instantiate From Pool {type}");
            var popUpNew = Instantiate(prefab as GameObject, parentObject);
            return popUpNew.GetComponent<T>();
        }

       
    }
    private void EnqueToPoolPrefabs<T>(T popUp){

        var type = popUp.GetType();
        var exist = poolListPrefab.Exists(x => x.GetType() == type);
        RemoveFromPoolSelf(popUp as PopUp);

        if (!exist)
        {
            (popUp as PopUp).gameObject.SetActive(false);
            poolListPrefab.Add(popUp as PopUp);
            Tools.Log($"Enqued From Pool {nameof(popUp)}");

        }

    }

    #endregion


    public List<PopUp> poolList = new List<PopUp>();
    public GameObject BlurUI_Prefab;
    UIBlur m_UiBlur;
    private bool m_WithBlur;
    private StateManager.State prevState;
    public bool IsPopupOpen => poolList.Count > 0;



    public void ShowQueue(UnityAction qeueuMessage) {
        if (poolList.Count > 0) {

            EnqueueAction(qeueuMessage);    
        }
        else
        {

            qeueuMessage?.Invoke();
        }

    }
    public T Show<T>(GameObject prefab, Transform parent = null, bool withBlur = true, float FadeMoveY_Start = 50,float FadeMoveY_End = 0, float FadeSpeed = 0.2f,  bool KeepSameState = false, params GameObject[] skipBlureList)
    {
        Transform parentObject;
        if (parent == null) parentObject = this.transform;
        else { parentObject = parent; }
        m_WithBlur = withBlur;


        if (m_WithBlur)
        {
            SkipBlureList(skipBlureList, false);
            ShowBlur(parentObject);
            SkipBlureList(skipBlureList, true);
        }


        T popUp = DequeFromPoolPrefabs<T>(prefab, parentObject)  ;  
        
        RectTransform viewRect = (popUp as PopUp).GetComponent<RectTransform>();
        CanvasGroup MainInfoGroup = (popUp as PopUp).GetComponent<CanvasGroup>();

        Sequence s = DOTween.Sequence();
        s.SetId(popUp).SetUpdate(true);
        s.Join(viewRect?.DOAnchorPosY(FadeMoveY_End, FadeSpeed).From(Vector2.one * FadeMoveY_Start).SetEase(Ease.OutBack));
        s.Join(MainInfoGroup?.DOFade(1, FadeSpeed));        //All prefabs should implement interface WindowUI


        (popUp as PopUp).Show(() => Hide(popUp as PopUp,FadeMoveY_Start,FadeSpeed));
        (popUp as PopUp).OnCompleteBase = () => Hide(popUp as PopUp, FadeMoveY_Start, FadeSpeed);
        (popUp as PopUp).blureUsed = withBlur;

        AddToPool(popUp as PopUp, KeepSameState);

      
        

        return popUp; 
    }

   
    public T ShowSimple<T>(GameObject prefab, Transform parent = null, float FadeInSpeed = 0.1f, float FadeOutSpeed = 0f, bool KeepSameState = false)
    {
        Transform parentObject;
        if (parent == null) parentObject = this.transform;
        else { parentObject = parent; }
        GameManager.Instance.InteractionState = InteractionState.UI;



        T popUp = DequeFromPoolPrefabs<T>(prefab, parentObject);
        RectTransform viewRect = (popUp as PopUp).GetComponent<RectTransform>();
        CanvasGroup MainInfoGroup = (popUp as PopUp).GetComponent<CanvasGroup>();


        Sequence s = DOTween.Sequence();
        s.SetUpdate(isIndependentUpdate: true);
        s.SetId(popUp).SetUpdate(true);
        s.Join(viewRect?.DOAnchorPosY(0, 0));
        s.Join(MainInfoGroup?.DOFade(1, FadeInSpeed));
        
        //All prefabs should implement interface WindowUI
        (popUp as PopUp).Show(() => FastHide(popUp as PopUp, FadeOutSpeed));
        (popUp as PopUp).OnCompleteBase = () => FastHide(popUp as PopUp, FadeOutSpeed);
        (popUp as PopUp).blureUsed = false;


        AddToPool(popUp as PopUp, KeepSameState);
        return (popUp);
    }


 
    public void SkipBlureList(GameObject[] skipBlureList, bool enable)
    {

        if (skipBlureList == null) return;

        foreach (GameObject item in skipBlureList)
        {
            item.SetActive(enable);
        }

    }
    public void ShowBlur(Transform parent = null, bool isCustom = false, float Delay = 0f)
    {
        Transform parentObject;
        if (parent == null) parentObject = this.transform;
        else { parentObject = parent; }

        
        if (m_UiBlur != null)
        {
            m_UiBlur.transform.SetParent(parentObject);
            if (isCustom) m_UiBlur.transform.SetAsFirstSibling(); else m_UiBlur.transform.SetAsLastSibling();
            m_UiBlur.Initialize(Delay);
          
        }
        else
        {

            m_UiBlur = Instantiate(BlurUI_Prefab, parentObject).GetComponent<UIBlur>();
            if (isCustom) m_UiBlur.transform.SetAsFirstSibling(); else m_UiBlur.transform.SetAsLastSibling();
            m_UiBlur.Initialize(Delay);
         
        }


    }
    public void ClearBlur()
    {
        if (m_UiBlur != null) m_UiBlur.ShowBluredImage(false);
    }



    public void CloseAllPopUps(bool keepSameState = false)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (poolList[i] != null)
                FastHide(poolList[i]);
        }
        GameManager.Instance.InteractionState = InteractionState.INGAME;
    }
    public void Hide(PopUp view, float FadeMoveYPos = 50, float FadeSpeed = 0.1f)
    {
        if (view == null) return;
        GameManager.Instance.InteractionState = InteractionState.INGAME;
        if (view.blureUsed) ClearBlur();
        RectTransform viewRect = (view).GetComponent<RectTransform>();
        CanvasGroup MainInfoGroup = (view).GetComponent<CanvasGroup>();



        view?.GetComponentInChildren<UIBlur>()?.gameObject.SetActive(false);
        Sequence s = DOTween.Sequence();
        s.SetId(view).SetUpdate(true);
        s.Join( MainInfoGroup?.DOFade(0, FadeSpeed));
        s.Join( viewRect?.DOAnchorPosY(-FadeMoveYPos, FadeSpeed).SetEase(Ease.InFlash));
        s.OnComplete(() => {
            DestroyOnComplete(view); 
        });
    }
    public void FastHide(PopUp view,float fadeOutSpeed = 0, bool keepSameState = false)
    {
        if (view == null) return;
        if (view.blureUsed) ClearBlur();
        CanvasGroup MainInfoGroup = (view).GetComponent<CanvasGroup>();
        if(!keepSameState)  GameManager.Instance.InteractionState = InteractionState.INGAME;

        Sequence s = DOTween.Sequence();
        s.SetId(view).SetUpdate(true);
        s.Join(MainInfoGroup?.DOFade(0, fadeOutSpeed));
        s.OnComplete(() => {
            DestroyOnComplete(view);
        });
       

      

    }
    public void DestroyOnComplete<T>(T view)
    {
        if(view == null) return;
        DOTween.Kill(view);

        if ((view as PopUp).Poolable)
        {

            EnqueToPoolPrefabs(view);
        }
        else {
            Destroy((view as PopUp).gameObject);
        }

       

    }


    //Pooling Functions
    public void AddToPool(PopUp view, bool keepSameState = false)
    {
        view.ignoreStateChange = keepSameState;

        if (poolList.Count == 0) {
            prevState = StateManager.Instance.currentState;
           
            if(!keepSameState)
            StateManager.Instance.SetState(StateManager.State.uiView);
        }

        poolList.Add(view);

      
    }
    public void RemoveFromPoolSelf(PopUp view ) {
        if(!poolList.Contains(view)) return;

        poolList.Remove(view);

        //RemoveFromPool(view);
        if (poolList.Count == 0)
        {

            DequeueNext();
            BackButtonReset();
          if(!view.ignoreStateChange)  StateManager.Instance.SetState(prevState);

        }


    }

    public void CloseLastPopUp() {
        if(poolList==null) return;

        if (poolList.Count > 0)
        {
            (poolList[poolList.Count - 1])?.OnCompleteBase();
            
        }
        
    }
    public void BackButtonReset() {

       
    }




    #region QUEUE MANAGEMENT
    private Queue<UnityAction> m_MainQueue = new Queue<UnityAction>();
    public int QueueCount=0;


    public void EnqueueAction(UnityAction action)
    {
        m_MainQueue.Enqueue(action);
        QueueCount = m_MainQueue.Count;
    }

    public void DequeueNext()
    {
        UnityAction queuedAction = m_MainQueue.Count > 0 ? m_MainQueue.Dequeue() : null;
        queuedAction?.Invoke();
        QueueCount = m_MainQueue.Count;
    }
    #endregion



  
}
