using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ANode
{
    public float totalCost;
    public float directCost;
    public Vector2Int location;
    public ANode parentNode;

    public ANode(ANode parent, Vector2Int goal, Vector2Int point, float cost)
    {
        directCost = cost;
        location = point;
        parentNode = parent;
        totalCost = directCost + Vector2.Distance(goal, location);
    }

    public bool CheckLocation(Vector2Int other)
    {
        return other.x == location.x && other.y == location.y;
    }
}
