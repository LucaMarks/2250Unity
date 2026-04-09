using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Active Quests")]
    public Quest currentQuest;
    public Quest secondaryQuest;

    [Header("UI")]
    public QuestObjectiveUI objectiveUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateObjectiveUI();
    }

    private void Update()
    {
        UpdateQuest(currentQuest);
        UpdateQuest(secondaryQuest);
        UpdateObjectiveUI();
    }

    public void StartQuest(Quest quest)
    {
        if (quest == null)
            return;

        if (quest == currentQuest || quest == secondaryQuest)
        {
            quest.StartQuest();
            UpdateObjectiveUI();
            return;
        }

        if (currentQuest == null || currentQuest.isCompleted)
        {
            currentQuest = quest;
        }
        else if (secondaryQuest == null || secondaryQuest.isCompleted)
        {
            secondaryQuest = quest;
        }
        else
        {
            Debug.LogWarning("QuestManager already has two active quests.");
            return;
        }

        quest.StartQuest();
        UpdateObjectiveUI();
    }

    public void ClearCurrentQuest()
    {
        currentQuest = null;
        UpdateObjectiveUI();
    }

    public void ClearSecondaryQuest()
    {
        secondaryQuest = null;
        UpdateObjectiveUI();
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveUI == null)
            return;

        string text = BuildObjectiveText();

        if (string.IsNullOrEmpty(text))
        {
            objectiveUI.HideObjective();
            return;
        }

        objectiveUI.SetObjectiveText(text);
    }

    private void UpdateQuest(Quest quest)
    {
        if (quest == null)
            return;

        if (!quest.hasStarted || quest.isCompleted)
            return;

        quest.UpdateQuest();
    }

    private string BuildObjectiveText()
    {
        string first = GetQuestText(currentQuest);
        string second = GetQuestText(secondaryQuest);

        if (string.IsNullOrEmpty(first))
            return second;

        if (string.IsNullOrEmpty(second))
            return first;

        return first + "\n" + second;
    }

    private string GetQuestText(Quest quest)
    {
        if (quest == null || !quest.hasStarted)
            return string.Empty;

        if (quest.isCompleted)
            return "Quest Complete: " + quest.questName;

        return quest.GetObjectiveText();
    }
}
