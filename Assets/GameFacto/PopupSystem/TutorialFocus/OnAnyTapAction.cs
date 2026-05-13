using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    if (!initialized)
        return;

    delay -= Time.deltaTime;

    if (delay > 0)
        return;

    if (Input.GetMouseButtonDown(0))
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()&& EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>() != null)
        {
            return;
        }

        OnClose?.Invoke();
        initialized = false;
    }
}
}
