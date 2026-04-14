using System;
using UnityEngine;

public abstract class BaseQuestObjective : IQuest
{
    public string Description { get; protected set; }
    public bool IsCompleted { get; protected set; }

    public event Action OnCompleted;

    protected BaseQuestObjective(string description)
    {
        Description = description;
    }

    public virtual void StartObjective()
    {
        IsCompleted = false;
    }

    public virtual void CompleteObjective()
    {
        IsCompleted = true;
        OnCompleted?.Invoke();
    }
}



public interface IQuest
{
    string Description { get; }
    bool IsCompleted { get; }

    void StartObjective();
    void CompleteObjective();
}
