using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Base Movement Stats")]
    protected Actor actor;

    //Pathing
    [SerializeField] protected AStar pathfinder;
    [SerializeField] protected int pathIndex;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float runMultiplier;
    protected Vector3 momentum;
    protected bool running;
    protected Vector3 spawnPoint;

    void Awake()
    {
        pathfinder = new AStar();
    }

    protected virtual void Initialize() 
    {
        
    }

    public void RebuildPathfinding(Vector2Int destination, bool forceIncludeEnding = false)
    {
        pathfinder.BuildPath(MyLocation(), destination, forceIncludeEnding);
        pathIndex = 0;
    }

    public bool CheckWorkPath(Vector2Int destination)
    {
        AStar testPath = new AStar();

        if(!testPath.BuildPath(MyLocation(), destination, true))
        {
            return false;
        }

        pathIndex = 0;
        pathfinder.BuildPath(MyLocation(), destination, true);
        return true;
    }

    public bool AtWorkSite(Vector2Int desitination)
    {
        Vector3 myLocation = transform.position;
        Vector3 nextPoint = GetPoint();

        if (Vector3.Distance(myLocation, nextPoint) <= 0.2f)
        {
            return pathfinder.CheckPoint(pathIndex + 1, MyLocation()) == desitination;
        }

        return false;
    }

    public bool PathAvailable()
    {
        return !pathfinder.DestinationReached(pathIndex);
    }

    public Vector3 NextPath()
    {
        Vector3 myLocation = transform.position;
        Vector3 nextPoint = GetPoint();

        if(Vector3.Distance(myLocation, nextPoint) <= 0.2f)
        {
            pathIndex++;
            nextPoint = GetPoint();
        }

        return nextPoint - myLocation;
    }

    protected Vector3 GetPoint()
    {
        Vector2 nextPoint = pathfinder.CheckPoint(pathIndex, MyLocation());
        return new Vector3(nextPoint.x + 0.5f, 1f, nextPoint.y + 0.5f);
    }

    public Vector2Int MyLocation()
    {
        return new Vector2Int(MathFun.Floor(transform.position.x), MathFun.Floor(transform.position.z));
    }

    public virtual void SetStats(Actor whoAmI, ActorStats stats)
    {
        actor = whoAmI;
        moveSpeed = stats.moveSpeed;
        runMultiplier = stats.runMultiplier;

    }

    protected virtual void GameLoop() 
    {
        float speed = moveSpeed;
        if (running) speed *= runMultiplier;

        transform.position += momentum * speed * Time.deltaTime;

        momentum = Vector3.zero;
    }

    public void SetSpawn(Vector2 point, bool relocate = true)
    {
        spawnPoint = new Vector3(point.x, 1f, point.y);
        if (relocate) transform.position = spawnPoint;
    }

    public void SetSpawn(Vector3 point, bool relocation = true)
    {
        SetSpawn(new Vector2(point.x, point.z), relocation);
    }

    public void Teleport(Vector3 point)
    {
        transform.position = point;
    }

    public void SetDirection(Vector3 direction)
    {
        momentum = direction.normalized;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, momentum, moveSpeed, 0f);
        Debug.DrawRay(transform.position, newDirection, Color.red);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    // Start is called before the first frame update
    void Start()
    {        
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();
    }
}
