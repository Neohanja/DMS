using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomBlueprint", menuName = "Neohanja/Room Blueprint")]
public class RoomBlueprint : ScriptableObject
{
    public string roomName;
    public int roomWidth;
    public int roomHeight;

    public bool saveRoom;

    public byte mainFloor;
    public byte mainWall;

    public List<BlueprintBlockPlacement> roomTiles;
    public List<Vector2Int> entryPoints;

    public bool eraseRoom;

    [HideInInspector] public Vector2 scroll;

    private void OnValidate()
    {
        if (roomWidth < 1) roomWidth = 1;
        if (roomHeight < 1) roomHeight = 1;

        if (saveRoom)
        {
            Debug.Log(roomName + " saved!");
            saveRoom = false;
        }

        if (entryPoints == null) entryPoints = new List<Vector2Int>();
        if (roomTiles == null) roomTiles = new List<BlueprintBlockPlacement>();

        if(eraseRoom)
        {
            roomTiles = new List<BlueprintBlockPlacement>();
            entryPoints = new List<Vector2Int>();
            eraseRoom = false;
        }        
    }
}

[System.Serializable]
public class BlueprintBlockPlacement : System.IEquatable<BlueprintBlockPlacement>, System.IComparable<BlueprintBlockPlacement>
{
    public int x;
    public int y;
    public byte floorTile;
    public byte wallTile;
    public int wallHeight;
    public int editorIndex;

    public BlueprintBlockPlacement(int xPoint, int yPoint, byte floor, byte wall, int height, int index)
    {
        x = xPoint;
        y = yPoint;
        floorTile = floor;
        wallTile = wall;
        wallHeight = height;
        editorIndex = index;
    }

    public BlueprintBlockPlacement(Vector2Int loc, BlueprintBlockPlacement tile)
    {
        x = loc.x;
        y = loc.y;
        floorTile = tile.floorTile;
        wallTile = tile.wallTile;
        wallHeight = tile.wallHeight;
        editorIndex = tile.editorIndex;
    }

    public BlueprintBlockPlacement(int xPoint, int yPoint) 
        : this(xPoint, yPoint, 0, 0, 0, 0) { }
    public BlueprintBlockPlacement(Vector2Int loc, byte floor, byte wall, int height, int index)
        : this(loc.x, loc.y, floor, wall, height, index) { }

    public int CompareTo(BlueprintBlockPlacement other)
    {
        if (other.y < y) return 1;
        if (other.y > y) return -1;
        if (other.x < x) return 1;
        if (other.x > x) return -1;

        Debug.Log("Duplicate X/Y coordinates found in Blueprint List");
        return 0;
    }

    public bool Equals(BlueprintBlockPlacement other)
    {
        return other.x == x && other.y == y;
    }
}

[System.Serializable]
public class RoomPlacement
{
    public int startX;
    public int startY;
    public RoomBlueprint room;

    public RoomPlacement(int x, int y, RoomBlueprint blueprint)
    {
        startX = x; 
        startY = y;
        room = blueprint;
    }
}