using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class OnAnyTapAction : MonoBehaviour
{
    public UnityAction OnClose;
    public float delay = 0;
    bool initialized = false;
    public void SetOnClose(UnityAction OnCloseAction, float minDelay = 0)
    {
        delay = minDelay;
        OnClose = OnCloseAction;
        initialized = true;


    }

    public void RemoveAllActions() {
        initialized = false;
        OnClose = null;
    }
    public void Update()
    {
        if (!initialized) return; 
        delay-=Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && delay<=0)
        {
            OnClose?.Invoke();
            initialized = false;
        }

        if(Input.GetMouseButtonUp(0) )
        {
            if (initialized) {
                OnClose?.Invoke();
                initialized = false;
            }
            
        }
    }
}
