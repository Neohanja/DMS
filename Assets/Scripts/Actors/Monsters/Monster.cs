using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Actor
{
    public Monster(ActorStats characterSheet) : base(characterSheet)
    {

    }

    protected override void BuildStateMachine()
    {
        stateMachine.AddState(State.StateID.Idle, new MonIdle(this, actorController));
        stateMachine.AddState(State.StateID.Wander, new MonWander(this, actorController));
    }
}
