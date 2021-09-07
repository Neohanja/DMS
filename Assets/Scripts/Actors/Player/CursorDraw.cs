using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDraw
{
    static readonly int tRows = 4;
    static readonly float tSize = 0.25f;

    GameObject cursorOverlay;
    MeshFilter mesh;
    MeshRenderer render;

    CursorIcons[] symbols;
    List<TaskLocation> taskQueue;
    TaskManager playerTaskManager;

    public CursorDraw(Material cursorMat, CursorIcons[] icons, TaskManager pc)
    {
        cursorOverlay = new GameObject("Player Cursor Overlay");
        mesh = cursorOverlay.AddComponent<MeshFilter>();
        render = cursorOverlay.AddComponent<MeshRenderer>();

        render.material = cursorMat;
        symbols = icons;
        playerTaskManager = pc;
        taskQueue = new List<TaskLocation>();
    }

    public void AddTask(Vector3Int location, int task)
    {
        location.y = World.WorldMap.GetHeight(location);

        TaskLocation newTask = new TaskLocation(location, task);
        bool removeTask = symbols[task].cancelActions;

        if (taskQueue.Contains(newTask))
        {
            int index = taskQueue.IndexOf(newTask);

            if (taskQueue[index].taskID == newTask.taskID || symbols[task].cancelActions)
            {
                taskQueue.Remove(newTask);
                removeTask = true;
            }
            else
            {
                taskQueue[index].taskID = newTask.taskID;
            }
        }
        else if(!symbols[task].cancelActions)
        {
            taskQueue.Add(newTask);
        }

        playerTaskManager.NewTask(newTask, removeTask);

        RebuildSymbolMap();
    }

    void RebuildSymbolMap()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> colorMap = new List<Color>();
        List<Vector2> uvMap = new List<Vector2>();
        int vCount = 0;

        foreach(TaskLocation task in taskQueue)
        {
            verts.Add(task.location + new Vector3(0, 0.01f, 0));
            verts.Add(task.location + new Vector3(0, 0.01f, 1));
            verts.Add(task.location + new Vector3(1, 0.01f, 1));
            verts.Add(task.location + new Vector3(1, 0.01f, 0));

            float x = symbols[task.taskID].actionTextureID % tRows;
            float y = symbols[task.taskID].actionTextureID / tRows;

            x *= tSize;
            y = 1f - (y * tSize) - tSize;

            uvMap.Add(new Vector2(x, y));
            uvMap.Add(new Vector2(x, y + tSize));
            uvMap.Add(new Vector2(x + tSize, y + tSize));
            uvMap.Add(new Vector2(x + tSize, y));

            colorMap.Add(symbols[task.taskID].actionColor);
            colorMap.Add(symbols[task.taskID].actionColor);
            colorMap.Add(symbols[task.taskID].actionColor);
            colorMap.Add(symbols[task.taskID].actionColor);

            tris.Add(vCount);
            tris.Add(vCount + 1);
            tris.Add(vCount + 2);

            tris.Add(vCount + 2);
            tris.Add(vCount + 3);
            tris.Add(vCount);

            vCount += 4;
        }

        mesh.mesh.Clear();
        mesh.mesh.vertices = verts.ToArray();
        mesh.mesh.uv = uvMap.ToArray();
        mesh.mesh.colors = colorMap.ToArray();
        mesh.mesh.triangles = tris.ToArray();
        mesh.mesh.RecalculateNormals();
    }
}

[System.Serializable]
public class TaskLocation : System.IComparable<TaskLocation>, System.IEquatable<TaskLocation>
{
    public Vector3Int location;
    public int taskID;
    public int priority;

    public TaskLocation(Vector3Int point, int id, int importance = 5)
    {
        location = point;
        taskID = id;
        priority = importance;
    }

    public int CompareTo(TaskLocation other)
    {
        if (priority < other.priority) return -1;
        if (priority > other.priority) return 1;
        return 0;
    }

    public bool Equals(TaskLocation other)
    {
        return other.location.x == location.x && other.location.z == location.z;
    }
}