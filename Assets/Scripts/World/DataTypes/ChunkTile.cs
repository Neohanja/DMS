using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkTile
{
    byte floorID;
    byte wallID;
    int height;
    bool visible;
    int lightLevel;

    ChunkTile[] neighbor;

    public ChunkTile(byte floor, byte wall, int stacked, int lit, bool tileVisible)
    {
        floorID = floor;
        wallID = wall;
        height = stacked;
        lightLevel = lit;
        visible = tileVisible;
        neighbor = new ChunkTile[4];
    }

    public float GetLightLevel
    {
        get
        {
            if (!visible) return 0f;

            return lightLevel / 15f;
        }
    }

    public int GetHeight
    {
        get
        {
            if (wallID == 0) return 1;
            return height;
        }
    }

    public byte GetTile(int y)
    {
        if (y <= 0) return floorID;
        if (y < height) return wallID;
        return 0;
    }

    public void ChangedTile(byte floor, byte wall, int tall)
    {
        floorID = floor;
        wallID = wall;
        height = tall;
    }

    public void AttachTile(Chunk.Direction direction, ChunkTile tile, bool outdoors)
    {
        int index = (int)direction;

        if (neighbor[index] == tile) return;
        neighbor[index] = tile;
        neighbor[index].AttachTile((Chunk.Direction)((index + 2) % 4), this, outdoors);

        if (visible && !outdoors)
        {
            int coverage = 0;

            for (int i = 0; i < 4; ++i)
            {
                if (neighbor[i] == null || worldMap.worldGen.tiles[neighbor[i].GetTile(2)].skipDraw) break;

                coverage++;
            }

            if (coverage >= 4) visible = false;
        }
    }

    static World worldMap { get { return World.WorldMap; } }
}