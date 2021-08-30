using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public List<TaskListing> taskList;
    public List<TaskListing> activeTasks;
    public List<Actor> subscribers;
    public List<string> testNames;

    void Awake()
    {
        taskList = new List<TaskListing>();
        activeTasks = new List<TaskListing>();
        subscribers = new List<Actor>();
        testNames = new List<string>();
    }

    void Update()
    {
        /*
        if(taskList.Count > activeTasks.Count && subscribers.Count > 0)
        {
            int actorIndex = 0;
            int taskIndex = 0;
        }
        */
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


    public enum TaskID
    {
        Attack,
        Mine,
        Build
    }


    [System.Serializable]
    public class TaskListing : System.IComparable<TaskListing>
    {
        public Vector2Int location;
        public TaskID taskType;
        public int priority;
        public byte taskID;
        public Actor assignedTo;

        public int CompareTo(TaskListing other)
        {
            if (other.priority < priority) return -1;
            if (other.priority > priority) return 1;
            return 0;
        }
    }
}
