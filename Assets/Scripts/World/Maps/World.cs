using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    public static World WorldMap { private set; get; }

    [Header("World Data Settings")]
    public RanGen worldRNG;
    public int chunkSize;
    public int worldSeed;
    public bool randomSeed;
    public Dictionary<Vector2Int, Chunk> chunkMap;
    protected Dictionary<Vector2Int, List<TileMod>> addChunkMods;
    public List<Chunk> activeChunks;
    public List<TileMod> moddedTiles;

    [Header("Material Settings")]
    public Material terrainMat;
    public int textureRows;
    public WorldGenSettings worldGen;
    public int tRows { get { return textureRows; } }
    public float tSize { get { return 1f / tRows; } }

    [Header("Debug Settings")]
    public bool rebuildMap;
    [Range(2,15)]
    public int maxRooms;
    [Range(0.05f, 1f)]
    public float globalLight;
    protected float previousLight;
    public bool debugOn;
    [HideInInspector] public bool genOpen;

    void Awake()
    {
        if (WorldMap != null && WorldMap != this)
        {
            Destroy(gameObject);
        }
        else
        {
            WorldMap = this;
        }

        if(randomSeed)
        {
            worldSeed = RanGen.PullNumber(42, MathFun.EpochTime);
        }

        chunkMap = new Dictionary<Vector2Int, Chunk>();
        moddedTiles = new List<TileMod>();
        addChunkMods = new Dictionary<Vector2Int, List<TileMod>>();
        activeChunks = new List<Chunk>();
        worldRNG = new RanGen(worldSeed);
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildMap();
        Vector2Int startLoc = new Vector2Int(chunkSize / 2, chunkSize / 2);
        RoomJigsaw.BuildDungeon(startLoc, maxRooms, 6, 7);
        AIManager.ActorManager.SetPlayerBase();
        globalLight = 0.65f;
        Shader.SetGlobalFloat("GlobalLighting", globalLight);
    }

    public void BuildMap()
    {
        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                AddChunk(new Vector2Int(x, y));
            }
        }
    }

    public bool LocationBlocked(float x, float y, float z)
    {
        int chunkX = MathFun.Floor(x / chunkSize);
        int chunkY = MathFun.Floor(z / chunkSize);

        Vector2Int chunkID = new Vector2Int(chunkX, chunkY);

        if (!chunkMap.ContainsKey(chunkID)) AddChunk(chunkID);

        int posX = MathFun.Floor(x) - (chunkX * chunkSize);
        int posY = MathFun.Floor(y);
        int posZ = MathFun.Floor(z) - (chunkY * chunkSize);

        return worldGen.tiles[chunkMap[chunkID].GetTile(posX, posY, posZ)].solid;
    }

    protected Chunk.ChunkType GetChunkType(Vector2Int location)
    {
        //if (location.x == 0 && location.y == 0) return Chunk.ChunkType.Cave;
        if (location.y > 0) return Chunk.ChunkType.Outside;
        return Chunk.ChunkType.Underground;
    }

    public void ModTile(List<TileMod> modTile)
    {
        List<Vector2Int> chunksToAdd = new List<Vector2Int>();

        for (int i = 0; i < modTile.Count; ++i)
        {
            if (!chunksToAdd.Contains(modTile[i].chunkID))
            {
                chunksToAdd.Add(modTile[i].chunkID);
                addChunkMods.Add(modTile[i].chunkID, new List<TileMod>());
            }

            addChunkMods[modTile[i].chunkID].Add(modTile[i]);

            if (!debugOn || modTile[i].chunkID.x < 0 || modTile[i].chunkID.y < 0)
            {
                moddedTiles.Add(modTile[i]);
            }
        }

        for(int j = 0; j < chunksToAdd.Count; ++j)
        {
            CheckChunkMods(chunksToAdd[j]);
        }
    }

    protected void CheckChunkMods(Vector2Int location)
    {
        if (chunkMap != null && chunkMap.ContainsKey(location) && addChunkMods.ContainsKey(location))
        {
            chunkMap[location].ModTiles(addChunkMods[location]);
            addChunkMods.Remove(location);
        }
    }

    protected void AddChunk(Vector2Int location)
    {
        if (chunkMap.ContainsKey(location)) return;

        chunkMap.Add(location, new Chunk(location, GetChunkType(location)));

        activeChunks.Add(chunkMap[location]);

        for (int direction = 0; direction < 4; ++direction)
        {
            if (chunkMap.ContainsKey(location + Walk[direction]))
            {
                chunkMap[location].AttachChunk((Chunk.Direction)direction, chunkMap[location + Walk[direction]]);
            }
        }

        CheckChunkMods(location);
    }

    // Update is called once per frame
    void Update()
    {
        if (rebuildMap)
        {
            moddedTiles.Clear();
            worldRNG = new RanGen(worldSeed);

            for (int i = 0; i < activeChunks.Count; ++i)
            {
                activeChunks[i].RebuildMe();
            }

            Vector2Int startLoc = new Vector2Int(chunkSize / 2, chunkSize / 2);
            RoomJigsaw.BuildDungeon(startLoc, maxRooms, 6, 7);
            rebuildMap = false;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("World");
        }

        if (globalLight != previousLight)
        {
            Shader.SetGlobalFloat("GlobalLighting", globalLight);
            previousLight = globalLight;
        }
    }

    public Biome GetChunkBiome(Vector2Int position, Chunk.ChunkType chunkStyle)
    {
        switch (chunkStyle)
        {
            case Chunk.ChunkType.Cave:
            case Chunk.ChunkType.Underground:
                return worldGen.underworld[RanGen.PullNumber(position.x, position.y, worldSeed) % worldGen.underworld.Length];
            case Chunk.ChunkType.Outside:
                return worldGen.overworld[RanGen.PullNumber(position.x, position.y, worldSeed) % worldGen.overworld.Length];
        }

        return worldGen.defaultBiome;
    }

    public void UpdateGenSettings()
    {
        if (debugOn) rebuildMap = true;
    }

    void OnApplicationQuit()
    {
        Shader.SetGlobalFloat("GlobalLighting", 1f);
    }

    public static Vector2Int[] Walk = new Vector2Int[]
    {
        new Vector2Int ( 0,  1),
        new Vector2Int ( 1,  0),
        new Vector2Int ( 0, -1),
        new Vector2Int (-1,  0)
    };
}
