using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterBuilder
{
    public static ActorStats Monster(Racial race, int raceID)
    {
        ActorStats newStats = new ActorStats();

        newStats.actorName = "Test Subject";

        newStats.seed = Random.Range(0, int.MaxValue) + 1;
        newStats.raceIndex = raceID;
        newStats.moveSpeed = race.baseMoveSpeed;
        newStats.actorType = (int)Actor.ActorType.Monster;

        RanGen prng = new RanGen(newStats.seed);

        int x, y;

        do
        {
            x = prng.Roll(0, World.WorldMap.chunkSize - 1);
            y = prng.Roll(0, World.WorldMap.chunkSize - 1);
        } while (AIManager.ActorManager != null && AIManager.ActorManager.SpaceOccupied(x, y));

        newStats.spawnLoc[0] = x + 0.5f;
        newStats.spawnLoc[1] = 1f;
        newStats.spawnLoc[2] = y + 0.5f;

        return newStats;
    }
}
