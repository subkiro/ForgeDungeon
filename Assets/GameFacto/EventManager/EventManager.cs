
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{



    #region Alternative Events For Tutorials

    private Dictionary<string, UnityEvent> eventDictionary;


    public void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
            SubscribeOnGameEvents();
        }
    }

  

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (EventManager.Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            EventManager.Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (EventManager.Instance == null) return;
        UnityEvent thisEvent = null;
        if (EventManager.Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (EventManager.Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
    //
    private void SubscribeOnGameEvents()
    {
        //TutorialManager.OnReleaseFromTutorial += () => TriggerEvent("OnReleaseFromTutorial");
        //TutorialManager.OnSelectFromTutorial += () => TriggerEvent("OnSelectFromTutorial");
        //OnUnitDrop += (card,data,slot) => TriggerEvent("OnUnitDrop");
       // OnMergeSuccess += (card) => TriggerEvent("OnMergeSuccess");
    }
    #endregion
}
