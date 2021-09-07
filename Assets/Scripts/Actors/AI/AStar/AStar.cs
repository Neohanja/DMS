using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AStar
{
    protected static readonly int StraightCost = 10;
    protected static readonly int DiagonalCost = 15;

    protected Dictionary<Vector2Int, NodeStatus> nodeStatus;
    protected Dictionary<Vector2Int, float> nodeCosts;
    protected List<ANode> openList;

    [SerializeField] protected List<Vector2Int> path;

    public AStar()
    {
        nodeStatus = new Dictionary<Vector2Int, NodeStatus>();
        nodeCosts = new Dictionary<Vector2Int, float>();
        openList = new List<ANode>();
    }

    public bool BuildPath(Vector3 start, Vector3 end, bool includeEndRegardless)
    {
        Vector2Int startPoint = new Vector2Int(MathFun.Floor(start.x), MathFun.Floor(start.z));
        Vector2Int destination = new Vector2Int(MathFun.Floor(end.x), MathFun.Floor(end.z));
        
        path = FindPath(startPoint, destination, includeEndRegardless);
        return !(path == null);
    }

    public bool BuildPath(Vector2Int start, Vector2Int end, bool includeEndRegardless)
    {
        path = FindPath(start, end, includeEndRegardless);
        return !(path == null);
    }

    protected List<Vector2Int> FindPath(Vector2Int beginPoint, Vector2Int endPoint, bool includeEndRegardless)
    {
        ANode start = new ANode(null, endPoint, beginPoint, 0);
        openList.Clear();
        nodeStatus.Clear();
        nodeCosts.Clear();

        //Need to put a limit on how far we can check, due to infinite terrain, if a point is in an unaccessable cave,
        //but the AI has an entire world to explore, this will cause an infinite loop until one of the lists crash due
        //an out of memory error.
        float maxCostAllowed = 2 * Vector2.Distance(beginPoint, endPoint) * 100;

        AddNodeToOpen(start);

        while(openList.Count > 0)
        {
            ANode currentNode = openList[openList.Count - 1];

            if(currentNode.CheckLocation(endPoint))
            {
                List<Vector2Int> bestPath = new List<Vector2Int>();
                while (currentNode != null)
                {
                    bestPath.Insert(0, currentNode.location);
                    currentNode = currentNode.parentNode;
                }
                return bestPath;
            }

            openList.Remove(currentNode);
            nodeCosts.Remove(currentNode.location);

            foreach (ANode possibleNode in GetNeighbors(currentNode, endPoint, includeEndRegardless))
            {
                if (nodeStatus.ContainsKey(possibleNode.location))
                {
                    if (nodeStatus[possibleNode.location] == NodeStatus.Closed) continue;

                    if (possibleNode.totalCost >= nodeCosts[possibleNode.location]) continue;                    
                }

                if (possibleNode.totalCost >= maxCostAllowed) continue;

                AddNodeToOpen(possibleNode);
            }

            nodeStatus[currentNode.location] = NodeStatus.Closed;            
        }
            
        return null;
    }

    protected void AddNodeToOpen(ANode node)
    {
        int index = 0;
        float cost = node.totalCost;

        while(openList.Count > index && cost < openList[index].totalCost)
        {
            index++;
        }

        openList.Insert(index, node);

        if (nodeCosts.ContainsKey(node.location)) nodeCosts[node.location] = node.totalCost;
        else nodeCosts.Add(node.location, node.totalCost);

        if (nodeStatus.ContainsKey(node.location)) nodeStatus[node.location] = NodeStatus.Open;
        else nodeStatus.Add(node.location, NodeStatus.Open);
    }

    //IER = Include Ending Regardless

    protected List<ANode> GetNeighbors(ANode node, Vector2Int goal, bool IER)
    {
        int x = node.location.x;
        int y = node.location.y;

        List<ANode> list = new List<ANode>();

        bool[] Nesw = new bool[8]
        {
            !Blocked(x, y + 1),     //0
            !Blocked(x + 1, y),     //1
            !Blocked(x, y - 1),     //2
            !Blocked(x - 1, y),     //3
            !Blocked(x - 1, y + 1), //4
            !Blocked(x + 1, y + 1), //5
            !Blocked(x + 1, y - 1), //6
            !Blocked(x - 1, y - 1), //7
        };

        Nesw[4] = Nesw[4] && Nesw[0] && Nesw[3];
        Nesw[5] = Nesw[5] && Nesw[0] && Nesw[1];
        Nesw[6] = Nesw[6] && Nesw[2] && Nesw[1];
        Nesw[7] = Nesw[7] && Nesw[2] && Nesw[3];

        if (Nesw[0] || (IER && new Vector2Int(x, y+1) == goal))
        {
            list.Add(new ANode(node, goal, new Vector2Int(x, y + 1), StraightCost + node.directCost));
        }

        if (Nesw[1] || (IER && new Vector2Int(x + 1, y) == goal))
        {
            list.Add(new ANode(node, goal, new Vector2Int(x + 1, y), StraightCost + node.directCost));
        }

        if (Nesw[2] || (IER && new Vector2Int(x, y - 1) == goal))
        {
            list.Add(new ANode(node, goal, new Vector2Int(x, y - 1), StraightCost + node.directCost));
        }

        if (Nesw[3] || (IER && new Vector2Int(x - 1, y) == goal))
        {
            list.Add(new ANode(node, goal, new Vector2Int(x - 1, y), StraightCost + node.directCost));
        }

        if (Nesw[4])// || (IER && new Vector2Int(x - 1, y + 1) == goal)) <---Diagonal
        {
            list.Add(new ANode(node, goal, new Vector2Int(x - 1, y + 1), DiagonalCost + node.directCost));
        }

        if (Nesw[5])// || (IER && new Vector2Int(x + 1, y + 1) == goal)) <---Diagonal
        {
            list.Add(new ANode(node, goal, new Vector2Int(x + 1, y + 1), DiagonalCost + node.directCost));
        }

        if (Nesw[6])// || (IER && new Vector2Int(x + 1, y - 1) == goal)) <---Diagonal
        {
            list.Add(new ANode(node, goal, new Vector2Int(x + 1, y - 1), DiagonalCost + node.directCost));
        }

        if (Nesw[7])// || (IER && new Vector2Int(x - 1, y - 1) == goal)) <---Diagonal
        {
            list.Add(new ANode(node, goal, new Vector2Int(x - 1, y - 1), DiagonalCost + node.directCost));
        }
        

        return list;
    }

    public bool DestinationReached(int index)
    {
        return path == null || index >= path.Count;
    }

    public Vector2Int CheckPoint(int index, Vector2Int startPoint)
    {
        if (path == null) return startPoint;
        if (index >= path.Count) return path[path.Count - 1];
        return path[index];
    }

    protected bool Blocked(int x, int y)
    {
        return AIManager.ActorManager.SpaceOccupied(x, y);
    }

    public enum NodeStatus { Open, Closed }    
}