using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorStats
{
    public string actorName;
    public float[] transform;
    public float[] spawnLoc;

    public int seed;
    public int actorType;
    public int rngIndex;

    public float moveSpeed;
    public float runMultiplier;    

    public int raceIndex;


    public ActorStats(Vector3 spawn, int seedID, Actor.ActorType actorID, int raceID)
    {
        transform = new float[3] { spawn.x, spawn.y, spawn.z };
        spawnLoc = new float[3] { spawn.x, spawn.y, spawn.z };
        seed = seedID;
        actorType = (int)actorID;
        rngIndex = 1;
        raceIndex = raceID;
    }

    public ActorStats()
    {
        transform = new float[3];
        spawnLoc = new float[3];
        seed = 0;
        actorType = 0;
        rngIndex = 1;
        raceIndex = 0;
    }

    public void UpdatePosition(Vector3 position)
    {
        transform[0] = position.x;
        transform[1] = position.y;
        transform[2] = position.z;
    }
    
    public Vector3 Position
    {
        get { return new Vector3(transform[0], transform[1], transform[2]); }
    }

    public Vector3 SpawnPoint
    {
        get { return new Vector3(spawnLoc[0], spawnLoc[1], spawnLoc[2]); }
    }
}
