using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UIElements;

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
   
   
    public T ShowSimple<T>(GameObject prefab, Transform parent = null, float FadeInSpeed = 0.1f, float FadeOutSpeed = 0f)
    {
        Transform parentObject;
        if (parent == null) parentObject = this.transform;
        else { parentObject = parent; }

        


        T popUp = DequeFromPoolPrefabs<T>(prefab, parentObject);
        PopUp view = popUp as PopUp;
        view.prevInteractionState = GameManager.Instance.InteractionState;
        GameManager.Instance.InteractionState = InteractionState.UI;

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


        AddToPool(popUp as PopUp);
        return (popUp);
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
    public void FastHide(PopUp view,float fadeOutSpeed = 0, bool keepSameState = false)
    {
        if (view == null) return;
        CanvasGroup MainInfoGroup = (view).GetComponent<CanvasGroup>();
        GameManager.Instance.InteractionState = view.prevInteractionState;

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
    public void AddToPool(PopUp view)
    {

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
