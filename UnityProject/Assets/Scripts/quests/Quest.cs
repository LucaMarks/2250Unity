using UnityEngine;

public abstract class Quest : MonoBehaviour
{
    [Header("Quest Info")]
    public string questName;

    [TextArea(2, 5)]
    public string description;

    [Header("Quest State")]
    public bool hasStarted;
    public bool isCompleted;

    public virtual void StartQuest()
    {
        if (hasStarted || isCompleted)
            return;

        hasStarted = true;
        Debug.Log("Started quest: " + questName);
    }

    public virtual void UpdateQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        if (CheckIfComplete())
        {
            CompleteQuest();
        }
    }

    public abstract string GetObjectiveText();

    public abstract bool CheckIfComplete();

    public virtual void CompleteQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        if (!CheckIfComplete())
            return;

        isCompleted = true;
        GiveReward();

        Debug.Log("Completed quest: " + questName);
    }

    protected virtual void GiveReward()
    {
        Debug.Log("Reward given for quest: " + questName);
    }
}