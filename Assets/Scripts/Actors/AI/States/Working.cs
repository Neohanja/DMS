using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Working : State
{
    bool workComplete;
    float workTime = 5f;

    public Working(Actor identity, Movement movement)
    {
        currentState = StateID.Working;
        actor = identity;
        controller = movement;
        stateTimer = 3f;
        elapsedTime = 0f;
        workComplete = false;
    }

    public override void Activities()
    {
        elapsedTime += Time.deltaTime;

        if (controller.AtWorkSite(destination))
        {
            //Do Work Stuff
            if(elapsedTime >= workTime)
            {
                workComplete = true;
                actor.CompleteTask();
            }
        }
        else
        {
            if (elapsedTime >= stateTimer)
            {
                elapsedTime -= stateTimer;
                controller.CheckWorkPath(destination);
            }

            controller.SetDirection(controller.NextPath().normalized);
            if (controller.AtWorkSite(destination)) elapsedTime = 0f;
        }        
    }

    public override void MoveToState()
    {
        elapsedTime = 0f;
        workComplete = false;
    }

    public override StateID Reaction()
    {
        if (workComplete)
        {
            workComplete = false;
            return StateID.Idle;
        }

        return currentState;
    }
}
