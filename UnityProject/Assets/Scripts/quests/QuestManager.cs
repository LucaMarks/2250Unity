using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Current Quest")]
    public Quest currentQuest;

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
        if (currentQuest == null)
            return;

        if (!currentQuest.hasStarted || currentQuest.isCompleted)
            return;

        currentQuest.UpdateQuest();
        UpdateObjectiveUI();
    }

    public void StartQuest(Quest quest)
    {
        if (quest == null)
            return;

        currentQuest = quest;
        currentQuest.StartQuest();
        UpdateObjectiveUI();
    }

    public void ClearCurrentQuest()
    {
        currentQuest = null;
        UpdateObjectiveUI();
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveUI == null)
            return;

        if (currentQuest == null || !currentQuest.hasStarted)
        {
            objectiveUI.HideObjective();
            return;
        }

        if (currentQuest.isCompleted)
        {
            objectiveUI.SetObjectiveText("Quest Complete: " + currentQuest.questName);
            return;
        }

        objectiveUI.SetObjectiveText(currentQuest.GetObjectiveText());
    }
}