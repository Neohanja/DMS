using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonWander : State
{
    public MonWander(Actor identity, Movement movement)
    {
        currentState = StateID.Wander;
        actor = identity;
        controller = movement;
        stateTimer = 3f;
        elapsedTime = 0f;
        SetRandomDestination(10);
    }

    public override void Activities()
    {
        elapsedTime += Time.deltaTime;

        if(elapsedTime >= stateTimer)
        {
            elapsedTime -= stateTimer;
            controller.RebuildPathfinding(destination);
        }

        controller.SetDirection(controller.NextPath().normalized);
    }

    public override void MoveToState()
    {
        elapsedTime = 0f;
        SetRandomDestination(10);
    }

    public override StateID Reaction()
    {
        if (!controller.PathAvailable()) return StateID.Idle;

        return currentState;
    }
}
