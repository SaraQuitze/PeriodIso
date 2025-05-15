using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TaskSystem : MonoBehaviour
{
    public List<Task> allTasks = new List<Task>();
    public List<Task> currentTasks = new List<Task>();
    private int completedTasks;

    public void AssignTask(Task task)
    {
        currentTasks.Add(task);
    }

    public void CompleteTask(Task task)
    {
        currentTasks.Remove(task);
        completedTasks++;
        //EconomySystem.Instance.AddCoins(1);
    }
}

[System.Serializable]
public class Task
{
    public string taskName;
    public FloorType targetFloor;
    //public NPCType assigner;
    public bool isCompleted;
}
