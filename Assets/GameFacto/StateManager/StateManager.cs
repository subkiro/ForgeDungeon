using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateManager : Singleton<StateManager>
{
    public State currentState = State.Initialization;
    public State previousState = State.Initialization;
    public static UnityAction<State, State> OnStateChange;
    public bool ShowDebug = true;


  

    public void SetState(State state)
    {
        if (ShowDebug) Tools.Log("SetState: " + state);
        if (currentState == state) return;

        previousState = currentState;
        currentState = state;
        OnStateChange?.Invoke(currentState, previousState);
    }


    public enum State
    {
       Initialization, Fight,Deck, uiView
    }
}
