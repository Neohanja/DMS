using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected Movement controller;
    protected StateID currentState;
    protected Actor actor;

    protected float elapsedTime;
    protected float stateTimer;

    protected Vector2Int destination;
    protected Transform target;

    //Base functions
    public abstract void Activities();
    public abstract void MoveToState();
    public abstract StateID Reaction();

    protected float DistanceToDest()
    {
        return Vector2.Distance(
            new Vector2(controller.transform.position.x, controller.transform.position.z),
            new Vector2(destination.x, destination.y));
    }

    public void SetRandomDestination(int range)
    {
        int xP, yP;
        Vector2Int startLoc = actor.CurrentLocation();

        do
        {
            xP = startLoc.x + actor.Roll(-range, range);
            yP = startLoc.y + actor.Roll(-range, range);
        } while ((xP == startLoc.x && yP == startLoc.y) || AIManager.ActorManager.SpaceOccupied(xP, yP));

        destination = new Vector2Int(xP, yP);
        controller.RebuildPathfinding(destination);
    }

    public void SetDestination(Vector2Int point)
    {
        destination = point;
    }

    public Vector3 RandomDirection()
    {
        int quad = actor.Roll(1, 4);

        float xP = actor.Percent();
        float yP = MathFun.PointOnCircle(xP);

        switch (quad)
        {
            case 2:
                yP *= -1;
                break;
            case 3:
                yP *= -1;
                xP *= -1;
                break;
            case 4:
                xP *= -1;
                break;
        }

        return new Vector3(xP, 0f, yP);
    }

    //Helper Functions

    public enum StateID
    {
        None,
        Idle,
        Wander,
        Working
    }
}
