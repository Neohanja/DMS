using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<TaskListing> taskList;
    public List<Actor> subscribers;
    public List<string> testNames;
    public ToolActions playerToolOverlay;

    void Awake()
    {
        taskList = new List<TaskListing>();
        subscribers = new List<Actor>();
        testNames = new List<string>();
    }

    void Start()
    {
        playerToolOverlay = GetComponent<ToolActions>();
    }

    void Update()
    {
        //Assigns tasks

        for(int a = 0; a < subscribers.Count; ++a)
        {
            if(!subscribers[a].IsBusy)
            {
                for(int t = 0; t < taskList.Count; ++t)
                {
                    if(taskList[t].assignedTo == null)
                    {
                        if (subscribers[a].AssignTask(taskList[t]))
                        {
                            taskList[t].assignedTo = subscribers[a];
                            break;
                        }
                    }
                }
            }
        }

        for(int t = 0; t < taskList.Count; ++t)
        {
            if(taskList[t].assignedTo != null)
            {
                if(taskList[t].assignedTo.WhatsMyTask() != taskList[t])
                {
                    taskList[t].assignedTo = null;
                }
                else if(!taskList[t].assignedTo.IsBusy)
                {
                    taskList[t].assignedTo.CancelTask();
                }
            }
        }
    }

    public void AddListener(Actor minion)
    {
        if(!subscribers.Contains(minion))
        {
            subscribers.Add(minion);
            testNames.Add(minion.actorMain.name);
        }
    }

    public void RemoveListener(Actor minion)
    {
        if (subscribers.Contains(minion))
        {
            subscribers.Remove(minion);
            testNames.Remove(minion.actorMain.name);
        }
    }

    public void ReportCompletion(TaskListing tasking)
    {
        int height = World.WorldMap.GetHeight(new Vector3Int(tasking.location.x, 0, tasking.location.y));
        byte wall = World.WorldMap.GetBlock(new Vector3Int(tasking.location.x, 1, tasking.location.y));
        byte floor = World.WorldMap.GetBlock(new Vector3Int(tasking.location.x, 0, tasking.location.y));

        switch(tasking.taskID)
        {
            case 0:
                wall = 0;
                height = 1;
                break;
            case 1:
                RemoveListing(tasking);
                return;
            case 2:
                wall = 6;
                height = 3;
                break;
        }

        World.WorldMap.ModTile(new TileMod(tasking.location, floor, wall, height));
        RemoveListing(tasking);
    }

    void RemoveListing(TaskListing tasking)
    {
        if(playerToolOverlay != null)
        {
            playerToolOverlay.TaskOver(tasking);
        }

        taskList.Remove(tasking);
        taskList.Sort();
    }

    public void NewTask(TaskLocation tasking, bool clearTask)
    {
        TaskListing newTask = new TaskListing(tasking);

        if(taskList.Contains(newTask))
        {
            if(clearTask)
            {
                int index = taskList.IndexOf(newTask);

                if(taskList[index].assignedTo != null)
                {
                    taskList[index].assignedTo.CancelTask();
                }

                taskList.Remove(newTask);
            }
            else
            {
                int index = taskList.IndexOf(newTask);
                taskList[index].priority = newTask.priority;
                taskList[index].taskID = newTask.taskID;
            }
        }
        else if(!clearTask)
        {
            taskList.Add(newTask);
        }

        taskList.Sort();
    }


    [System.Serializable]
    public class TaskListing : System.IComparable<TaskListing>, System.IEquatable<TaskListing>
    {
        public Vector2Int location;
        public int priority;
        public int taskID;
        public Actor assignedTo;

        public TaskListing(TaskLocation tasking)
        {
            location = new Vector2Int(tasking.location.x, tasking.location.z);
            priority = tasking.priority;
            taskID = tasking.taskID;
            assignedTo = null;
        }

        public int CompareTo(TaskListing other)
        {
            if (other.priority < priority) return -1;
            if (other.priority > priority) return 1;
            return 0;
        }

        public bool Equals(TaskListing other)
        {
            return other.location.x == location.x && other.location.y == location.y;
        }
    }
}
