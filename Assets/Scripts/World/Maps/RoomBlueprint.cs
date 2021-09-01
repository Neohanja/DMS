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

    public List<BlueprintBlockPlacement> roomTiles;
    public List<Vector2Int> entryPoints;

    public bool eraseRoom;

    [HideInInspector] public Vector2 scroll;

    private void OnValidate()
    {
        if (roomWidth < 1) roomWidth = 1;

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
    public int editorIndex;

    public BlueprintBlockPlacement(int xPoint, int yPoint, int index)
    {
        x = xPoint;
        y = yPoint;
        editorIndex = index;
    }

    public BlueprintBlockPlacement(Vector2Int loc, BlueprintBlockPlacement tile)
    {
        x = loc.x;
        y = loc.y;
        editorIndex = tile.editorIndex;
    }

    public BlueprintBlockPlacement(int xPoint, int yPoint) 
        : this(xPoint, yPoint, 0) { }
    public BlueprintBlockPlacement(Vector2Int loc, int index)
        : this(loc.x, loc.y, index) { }

    public int CompareTo(BlueprintBlockPlacement other)
    {
        if (other.x < x) return 1;
        if (other.x > x) return -1;
        if (other.y < y) return 1;
        if (other.y > y) return -1;

        Debug.Log("Duplicate X/Y coordinates found in Blueprint List");
        return 0;
    }

    public bool Equals(BlueprintBlockPlacement other)
    {
        return other.x == x && other.y == y;
    }
}