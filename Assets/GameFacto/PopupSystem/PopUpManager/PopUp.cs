
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public  abstract class PopUp : MonoBehaviour
{
    public bool Poolable = true;
    public UnityAction OnCompleteBase;
    public bool blureUsed = false;
    public bool ignoreStateChange = false;
    public virtual void Show(UnityAction OnComplete) { 
    
    }

    public void RemoveFromPool()
    {
        
        PopUpManager.Instance.RemoveFromPoolSelf(this);
    }

   

    public virtual void OnDestroy()
    {
        RemoveFromPool();
    }

}
