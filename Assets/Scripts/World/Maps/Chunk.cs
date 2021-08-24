using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public GameObject chunkObj;
    protected MeshFilter chunkMesh;
    protected MeshRenderer chunkRender;
    protected MeshData chunkMeshData;
    protected MeshCollider chunkCollision;

    protected ChunkTile[,] chunkData;
    protected ANode[,] nodeMap;
    protected Vector2Int chunkCord;
    protected ChunkType chunkType;

    protected Chunk[] neighbors;

    public Chunk(Vector2Int location, ChunkType genType)
    {
        chunkCord = location;
        chunkType = genType;
        neighbors = new Chunk[4];

        chunkObj = new GameObject("Chunk");
        chunkObj.layer = 6;
        chunkObj.transform.SetParent(worldMap.transform);
        chunkObj.transform.position = new Vector3(location.x * cSize, 0f, location.y * cSize);

        chunkMesh = chunkObj.AddComponent<MeshFilter>();
        chunkRender = chunkObj.AddComponent<MeshRenderer>();
        chunkCollision = chunkObj.AddComponent<MeshCollider>();
        chunkMeshData = new MeshData(chunkMesh.mesh);
        chunkRender.material = worldMap.terrainMat;

        chunkData = new ChunkTile[cSize, cSize];

        RebuildMe();
    }

    public void RebuildMe()
    {
        BuildChunkData();
        BuildChunkMesh();
    }

    public void BuildChunkData()
    {
        bool[,] placementMap = CellAuto.CellMap(chunkCord * cSize, cSize, worldMap.worldSeed, worldMap.worldGen);
        float[,] hMap = GradNoise.GradMap(chunkCord * cSize, worldMap, worldMap.worldGen);
        Biome useBiome = worldMap.GetChunkBiome(chunkCord, chunkType);

        List<bool[,]> clutterMap = new List<bool[,]>();

        for(int i = 0; i < useBiome.biomeDecor.Length;++i)
        {
            clutterMap.Add(GradNoise.ClutterMap(chunkCord * cSize, worldMap, useBiome.biomeDecor[i]));
        }

        for (int x = 0; x < cSize; ++x)
        {
            for (int y = 0; y < cSize; ++y)
            {
                //Baseline stats
                byte floor = useBiome.groundTile;
                byte wall = useBiome.wallTile;
                int height = 3;

                if (chunkType.Equals(ChunkType.Cave) && !placementMap[x, y])
                {
                    wall = 0;
                    height = 1;
                }

                if (chunkType.Equals(ChunkType.Outside))
                {
                    height = MathFun.Round(useBiome.growth * hMap[x, y]);
                    if (height < 1) height = 1;
                }

                for (int c = 0; c < useBiome.biomeDecor.Length; ++c)
                {
                    if (clutterMap[c][x, y] && wall != 0)
                    {
                        wall = useBiome.biomeDecor[c].tile;
                        break;
                    }
                }

                chunkData[x, y] = new ChunkTile(floor, wall, height, 15, true);

                if (!worldMap.debugOn)
                {
                    //For now, if debug is on, I want to see the whole map.
                    if (x > 0) chunkData[x, y].AttachTile(Direction.West, chunkData[x - 1, y], chunkType == ChunkType.Outside);
                    if (y > 0) chunkData[x, y].AttachTile(Direction.South, chunkData[x, y - 1], chunkType == ChunkType.Outside);
                }
            }
        }
    }

    public void BuildChunkMesh()
    {
        for (int x = 0; x < cSize; ++x)
        {
            for (int z = 0; z < cSize; ++z)
            {
                for(int y = 0; y < chunkData[x,z].GetHeight; ++y)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    TileData currentTile = worldMap.worldGen.tiles[GetTile(position)];

                    if (!currentTile.skipDraw && currentTile.baseShape != null)
                    {
                        for(int face = 0; face < 6; ++face)
                        {
                            TileData nextTile = worldMap.worldGen.tiles[GetTile(position + Directional[face])];

                            if (nextTile.skipDraw || (face < 2 ? nextTile.drawBottom : nextTile.drawSurrounding))
                            {
                                chunkMeshData.AddFace(
                                    position,
                                    currentTile.baseShape.GetFace(face),
                                    currentTile.GetTextureIndex(face),
                                    chunkData[x, z].GetLightLevel);
                            }
                        }
                    }
                }
            }
        }

        chunkMeshData.ApplyMesh();
        chunkCollision.sharedMesh = chunkMesh.mesh;
    }

    public byte GetTile(Vector3Int position)
    {
        return GetTile(position.x, position.y, position.z);
    }

    public byte GetTile(int x, int y, int z)
    {
        if(z >= cSize)
        {
            if (neighbors[0] == null) return 0;
            return neighbors[0].GetTile(x, y, z - cSize);
        }

        if(x >= cSize)
        {
            if (neighbors[1] == null) return 0;
            return neighbors[1].GetTile(x - cSize, y, z);
        }

        if(z < 0)
        {
            if (neighbors[2] == null) return 0;
            return neighbors[2].GetTile(x, y, z + cSize);
        }

        if(x < 0)
        {
            if (neighbors[3] == null) return 0;
            return neighbors[3].GetTile(x + cSize, y, z);
        }

        return chunkData[x, z].GetTile(y);
    }

    public ChunkTile GetFullTileData(int x, int y)
    {
        if (y >= cSize)
        {
            if (neighbors[0] == null) return null;
            return neighbors[0].GetFullTileData(x, y - cSize);
        }

        if (x >= cSize)
        {
            if (neighbors[1] == null) return null;
            return neighbors[1].GetFullTileData(x - cSize, y);
        }

        if (y < 0)
        {
            if (neighbors[2] == null) return null;
            return neighbors[2].GetFullTileData(x, y + cSize);
        }

        if (x < 0)
        {
            if (neighbors[3] == null) return null;
            return neighbors[3].GetFullTileData(x + cSize, y);
        }

        return chunkData[x, y];
    }

    public void ModTiles(List<TileMod> tileMods)
    {
        for(int m = 0; m < tileMods.Count; ++m)
        {
            chunkData[tileMods[m].x, tileMods[m].y].ChangedTile(tileMods[m].floor, tileMods[m].wall, tileMods[m].height);
        }

        BuildChunkMesh();
    }

    public void AttachChunk(Direction direction, Chunk chunk)
    {
        int index = (int)direction;

        if (neighbors[index] == chunk) return;
        neighbors[index] = chunk;
        neighbors[index].AttachChunk((Direction)((index + 2) % 4), this);

        if (!worldMap.debugOn)
        {
            switch (direction)
            {
                case Direction.North:
                    for(int x = 0; x < cSize; ++x)
                    {
                        chunkData[x, cSize - 1].AttachTile(Direction.North, GetFullTileData(x, cSize), chunkType == ChunkType.Outside);
                    }
                    break;
                case Direction.East:
                    for (int y = 0; y < cSize; ++y)
                    {
                        chunkData[cSize - 1, y].AttachTile(Direction.East, GetFullTileData(cSize, y), chunkType == ChunkType.Outside);
                    }
                    break;
                case Direction.South:
                    for (int x = 0; x < cSize; ++x)
                    {
                        chunkData[x, 0].AttachTile(Direction.South, GetFullTileData(x, -1), chunkType == ChunkType.Outside);
                    }
                    break;
                case Direction.West:
                    for (int y = 0; y < cSize; ++y)
                    {
                        chunkData[0, y].AttachTile(Direction.West, GetFullTileData(-1, y), chunkType == ChunkType.Outside);
                    }
                    break;
            }
        }

        BuildChunkMesh();
    }

    protected int cSize { get { return World.WorldMap.chunkSize; } }
    protected World worldMap { get { return World.WorldMap; } }


    public static Vector3Int[] Directional = new Vector3Int[]
    {
        new Vector3Int ( 0,  1,  0),
        new Vector3Int ( 0, -1,  0),
        new Vector3Int ( 0,  0,  1),
        new Vector3Int ( 1,  0,  0),
        new Vector3Int ( 0,  0, -1),
        new Vector3Int (-1,  0,  0)
    };

    public enum Direction
    {
        North,
        East,
        South,
        West,
        Top,
        Bottom,
        None
    }

    public enum ChunkType
    {
        Cave,
        Underground,
        Outside
    }
}

[System.Serializable]
public class TileMod
{
    public int x;
    public int y;
    public Vector2Int chunkID;
    public byte floor;
    public byte wall;
    public int height;

    public TileMod(Vector2 location, byte floorID, byte wallID, int heightLevel)
    {
        int cX = MathFun.Floor(location.x / World.WorldMap.chunkSize);
        int cY = MathFun.Floor(location.y / World.WorldMap.chunkSize);

        chunkID = new Vector2Int(cX, cY);

        x = MathFun.Floor(location.x) - cX * World.WorldMap.chunkSize;
        y = MathFun.Floor(location.y) - cY * World.WorldMap.chunkSize;

        floor = floorID;
        wall = wallID;
        height = heightLevel;
    }
}