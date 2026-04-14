using System.Collections.Generic;

public class QuestChain
{
    private readonly List<BaseQuestObjective> objectives;
    private int currentIndex = 0;

    public BaseQuestObjective CurrentObjective =>
        currentIndex < objectives.Count ? objectives[currentIndex] : null;

    public bool IsCompleted => currentIndex >= objectives.Count;

    public QuestChain(List<BaseQuestObjective> objectives)
    {
        this.objectives = objectives;
    }

    public void StartQuest()
    {
        StartCurrentObjective();
    }

    private void StartCurrentObjective()
    {
        if (IsCompleted) return;

        var current = CurrentObjective;
        current.OnCompleted += MoveNext;
        current.StartObjective();
    }

    private void MoveNext()
    {
        CurrentObjective.OnCompleted -= MoveNext;
        currentIndex++;

        if (!IsCompleted)
            StartCurrentObjective();
        else
            UnityEngine.Debug.Log("Quest Completed!");
    }
}