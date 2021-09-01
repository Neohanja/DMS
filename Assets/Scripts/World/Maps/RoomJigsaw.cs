using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomJigsaw
{
    private static int wallHeight = 3;
    private static int floorHeight = 1;

    public static void BuildDungeon(Vector2Int startLoc, int maxRooms, byte wallID, byte floorID)
    {
        if (maxRooms <= 0) return;

        List<TileMod> addRooms = new List<TileMod>();
        List<Room> buildRooms = new List<Room>();

        int cX = startLoc.x;
        int cY = startLoc.y;

        RoomBlueprint currentRoom = GetRoom(0);
        buildRooms.Add(new Room(new Vector2Int(cX, cY), currentRoom.roomWidth - 1, currentRoom.roomHeight - 1));

        foreach (BlueprintBlockPlacement block in currentRoom.roomTiles)
        {
            if (block.editorIndex > 1)
            {
                addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), floorID, wallID, floorHeight));
            }
            else
            {
                addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), floorID, wallID, wallHeight));
            }
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

            while ((int)entrances[entryIndex].direction != (int)(exits[exitIndex].direction + 2) % 4)
            {
                roomPoints = RotateRoom(roomPoints, (rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight);
                entrances = RotateRoom(entrances, (rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight);
                rotated++;
            }

            cX = exits[exitIndex].basePoint.x;
            cY = exits[exitIndex].basePoint.y;

            switch (exits[exitIndex].direction)
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

            Room testRoom = new Room(new Vector2Int(cX, cY),
                ((rotated % 2 == 0) ? nextRoom.roomWidth : nextRoom.roomHeight) - 1,
                ((rotated % 2 == 0) ? nextRoom.roomHeight : nextRoom.roomWidth) - 1);
            bool useRoom = true;

            foreach (Room existingRoom in buildRooms)
            {
                if (existingRoom.WithinBounds(testRoom)) useRoom = false;
            }

            if (!useRoom)
            {
                //i--;
                Debug.Log("Woops! A room was built on a room.");
                //continue;
            }

            foreach (BlueprintBlockPlacement block in roomPoints)
            {
                if (block.editorIndex > 1)
                {
                    addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), floorID, wallID, floorHeight));
                }
                else
                {
                    addRooms.Add(new TileMod(new Vector2(cX + block.x, cY + block.y), floorID, wallID, wallHeight));
                }
            }

            entrances.RemoveAt(entryIndex);
            exits.RemoveAt(exitIndex);

            if (entrances.Count > 0)
            {
                foreach (Pathways addExit in entrances)
                {
                    addExit.basePoint.x += cX;
                    addExit.basePoint.y += cY;

                    exits.Add(addExit);
                }
            }
        }

        for(int i = 0; i < exits.Count; ++i)
        {
            TileMod checkTile = new TileMod(exits[i].basePoint, floorID, wallID, wallHeight);
            if(addRooms.Contains(checkTile))
            {
                int index = addRooms.IndexOf(checkTile);
                addRooms[index].height = checkTile.height;
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
        for (int i = 0; i < points.Count; ++i)
        {
            points[i].basePoint = RotatePoint(points[i].basePoint, width);
            points[i].direction = (Chunk.Direction)(((int)points[i].direction + 1) % 4);
        }

        return points;
    }

    private static List<BlueprintBlockPlacement> RotateRoom(List<BlueprintBlockPlacement> points, int width)
    {
        List<BlueprintBlockPlacement> roomPoints = new List<BlueprintBlockPlacement>();

        foreach (BlueprintBlockPlacement block in points)
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

    private static int RandomIndex(int count)
    {
        return World.WorldMap.worldRNG.RandomIndex(count);
    }

    private static RoomBlueprint GetRoom(int index = -1)
    {
        if (index == -1) index = RandomIndex(World.WorldMap.worldGen.pregenRooms.Length);

        return World.WorldMap.worldGen.pregenRooms[index];
    }


    private class Pathways
    {
        public Chunk.Direction direction;
        public Vector2Int basePoint;

        public Pathways(Chunk.Direction exitWay, Vector2Int location)
        {
            direction = exitWay;
            basePoint = location;
        }
    }

    private class Room
    {
        int left;
        int right;
        int top;
        int bottom;

        public Room(Vector2Int startPoint, int width, int height)
        {
            left = startPoint.x;
            right = startPoint.x + width;
            bottom = startPoint.y;
            top = startPoint.y + height;
        }

        public bool WithinBounds(Vector2Int point)
        {
            return point.x >= left && point.x <= right && point.y >= bottom && point.y <= top;
        }

        public bool WithinBounds(Room other, int padding = 0)
        {
            if (left == right || top == bottom) return false;
            if (other.left == other.right || other.top == other.bottom) return false;

            return WithinBounds(new Vector2Int(other.left + padding, other.bottom + padding)) ||
                   WithinBounds(new Vector2Int(other.left + padding, other.top - padding)) ||
                   WithinBounds(new Vector2Int(other.right - padding, other.bottom + padding)) ||
                   WithinBounds(new Vector2Int(other.right - padding, other.top - padding)) ||
                   other.WithinBounds(new Vector2Int(left + padding, bottom + padding)) ||
                   other.WithinBounds(new Vector2Int(left + padding, top - padding)) ||
                   other.WithinBounds(new Vector2Int(right - padding, bottom + padding)) ||
                   other.WithinBounds(new Vector2Int(right - padding, top - padding));
        }
    }
}