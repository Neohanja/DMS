using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State.StateID currentState;
    protected State.StateID previousState;
    protected Dictionary<State.StateID, State> stateLibrary;

    void Update()
    {
        if(stateLibrary == null)
        {
            Debug.Log(gameObject.name + "'s state machine library not built.");
            return;
        }

        currentState = stateLibrary[currentState].Reaction();
        
        if(currentState != previousState)
        {
            if(stateLibrary.ContainsKey(currentState))
            {
                stateLibrary[currentState].MoveToState();
                previousState = currentState;
            }
            else
            {
                currentState = previousState;
                Debug.Log(currentState.ToString() + " is not in " + gameObject.name + "'s state machine library. Reverting to " + previousState.ToString() + " state.");
            }
        }

        stateLibrary[currentState].Activities();
    }

    public void GiveTask(Vector2Int location)
    {
        if (stateLibrary.ContainsKey(State.StateID.Working))
        {
            stateLibrary[State.StateID.Working].SetDestination(location);
            currentState = State.StateID.Working;
        }
        else
        {
            Debug.Log(gameObject.name + " does not have a work function.");
        }
    }

    public void CancelTask()
    {
        currentState = State.StateID.Idle;        
    }

    public void AddState(State.StateID addedState, State newState)
    {
        if (stateLibrary == null)
        {
            stateLibrary = new Dictionary<State.StateID, State>();
            currentState = addedState;
        }
        else if (stateLibrary.ContainsKey(addedState)) //If we just made the state machine, we don't need to check this...
        {
            Debug.Log(addedState.ToString() + " is already in " + gameObject.name + "'s state machine library.");
            return;
        }

        stateLibrary.Add(addedState, newState);
    }
}
