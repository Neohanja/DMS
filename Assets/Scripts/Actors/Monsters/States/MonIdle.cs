using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonIdle : State
{
    public MonIdle(Actor identity, Movement movement)
    {
        currentState = StateID.Idle;
        actor = identity;
        controller = movement;
        MoveToState();
    }

    public override void Activities()
    {
        elapsedTime += Time.deltaTime;
    }

    public override void MoveToState()
    {
        stateTimer = actor.Roll(200, 500) / 100f;
        elapsedTime = 0f;
    }

    public override StateID Reaction()
    {
        if(elapsedTime >= stateTimer)
        {
            return StateID.Wander;
        }

        return currentState;
    }
}
