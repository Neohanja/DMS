using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomJigsaw
{
    public static void BuildStartDungeon(int maxRooms)
    {
        if (maxRooms <= 0) return;

        List<TileMod> addRooms = new List<TileMod>();

        int cX = World.WorldMap.chunkSize / 2 - 5;
        int cY = World.WorldMap.chunkSize / 2 - 5;        

        RoomBlueprint currentRoom = GetRoom(0);

        foreach(BlueprintBlockPlacement block in currentRoom.roomTiles)
        {
            addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), block.floorTile, block.wallTile, block.wallHeight));
        }

        List<Pathways> exits = GetExits(new Vector2Int(cX, cY), currentRoom);

        for (int i = 1; i < maxRooms; ++i)
        {
            if (exits.Count <= 0) break;

            RoomBlueprint nextRoom = GetRoom();
            List<BlueprintBlockPlacement> roomPoints = new List<BlueprintBlockPlacement>(nextRoom.roomTiles);
            List<Pathways> entrances = GetExits(new Vector2Int(0, 0), nextRoom);

            int exitIndex = RandomIndex(exits.Count);
            int entryIndex = RandomIndex(entrances.Count);

            if (entrances.Count == 0) continue;

            int rotated = 0;

            while((int)entrances[entryIndex].direction != (int)(exits[exitIndex].direction + 2) % 4)
            {
                roomPoints = RotateRoom(roomPoints, (rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight);
                entrances = RotateRoom(entrances, (rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight);
                rotated++;
            }

            cX = exits[exitIndex].basePoint.x;
            cY = exits[exitIndex].basePoint.y;

            switch(exits[exitIndex].direction)
            {
                case Chunk.Direction.North:
                    cY += 1;
                    cX -= entrances[entryIndex].basePoint.x;
                    break;
                case Chunk.Direction.East:
                    cY -= entrances[entryIndex].basePoint.y;
                    cX += 1;                    
                    break;
                case Chunk.Direction.South:
                    cY -= (rotated % 2 == 0) ? nextRoom.roomHeight : nextRoom.roomWidth;
                    cX -= entrances[entryIndex].basePoint.x;
                    break;
                case Chunk.Direction.West:
                    cY -= entrances[entryIndex].basePoint.y;
                    cX -= (rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight;
                    break;
            }

            foreach (BlueprintBlockPlacement block in roomPoints)
            {
                addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), block.floorTile, block.wallTile, block.wallHeight));
            }

            entrances.RemoveAt(entryIndex);
            exits.RemoveAt(exitIndex);

            if(entrances.Count > 0)
            {
                foreach(Pathways addExit in entrances)
                {
                    addExit.basePoint.x += cX;
                    addExit.basePoint.y += cY;

                    exits.Add(addExit);
                }
            }
        }

        World.WorldMap.ModTile(addRooms);
    }

    private static Vector2Int RotatePoint(Vector2Int point, int width)
    {
        int x = (point.x * 0) + (point.y * 1);
        int y = (point.x * -1) + (point.y * 0) + (width - 1);

        return new Vector2Int(x, y);
    }

    private static Vector2Int RotatePoint(int x, int y, int width)
    {
        return RotatePoint(new Vector2Int(x, y), width);
    }

    private static List<Pathways> RotateRoom(List<Pathways> points, int width)
    {
        for(int i = 0; i < points.Count; ++i)
        {
            points[i].basePoint = RotatePoint(points[i].basePoint, width);
            points[i].direction = (Chunk.Direction)(((int)points[i].direction + 1) % 4);
        }

        return points;
    }

    private static List<BlueprintBlockPlacement> RotateRoom(List<BlueprintBlockPlacement> points, int width)
    {
        List<BlueprintBlockPlacement> roomPoints = new List<BlueprintBlockPlacement>();

        foreach(BlueprintBlockPlacement block in points)
        {
            roomPoints.Add(new BlueprintBlockPlacement(RotatePoint(block.x, block.y, width), block));
        }

        return roomPoints;
    }

    private static List<Pathways> GetExits(Vector2Int start, RoomBlueprint currentRoom)
    {
        List<Pathways> exits = new List<Pathways>();

        foreach (Vector2Int point in currentRoom.entryPoints)
        {
            if (point.y == currentRoom.roomHeight - 1)
            {
                exits.Add(new Pathways(Chunk.Direction.North, point + start));
            }
            else if (point.x == currentRoom.roomWidth - 1)
            {
                exits.Add(new Pathways(Chunk.Direction.East, point + start));
            }
            else if (point.y == 0)
            {
                exits.Add(new Pathways(Chunk.Direction.South, point + start));
            }
            else if (point.x == 0)
            {
                exits.Add(new Pathways(Chunk.Direction.West, point + start));
            }
        }

        return exits;
    }

    private static int Roll(int min, int max)
    {
        return World.WorldMap.worldRNG.Roll(min, max);
    }

    private static int RandomIndex(int count)
    {
        return World.WorldMap.worldRNG.RandomIndex(count);
    }

    private static RoomBlueprint GetRoom(int index = -1)
    {
        if (index == -1) index = RandomIndex(World.WorldMap.worldGen.pregenRooms.Length);

        return World.WorldMap.worldGen.pregenRooms[index];
    }
}

public class Pathways
{
    public Chunk.Direction direction;
    public Vector2Int basePoint;

    public Pathways(Chunk.Direction exitWay, Vector2Int location)
    {
        direction = exitWay;
        basePoint = location;
    }
}