using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterBuilder
{
    public static ActorStats Monster(Racial race, int seedID, Vector2Int anchorPoint, int spawnRange)
    {
        ActorStats newStats = new ActorStats();

        newStats.seed = seedID;
        newStats.race = race;
        newStats.moveSpeed = race.baseMoveSpeed;
        newStats.actorType = (int)Actor.ActorType.Monster;

        RanGen prng = new RanGen(newStats.seed);

        if(race.genericNames != null && race.genericNames.Length > 0)
        {
            newStats.actorName = race.genericNames[prng.RandomIndex(race.genericNames.Length)];
        }
        else
        {
            newStats.actorName = race.raceName;
        }

        int x, y;

        do
        {
            x = anchorPoint.x + prng.Roll(0, spawnRange);
            y = anchorPoint.y + prng.Roll(0, spawnRange);
        } while (AIManager.ActorManager != null && AIManager.ActorManager.SpaceOccupied(x, y));

        newStats.spawnLoc[0] = x + 0.5f;
        newStats.spawnLoc[1] = 1f;
        newStats.spawnLoc[2] = y + 0.5f;

        return newStats;
    }
}
