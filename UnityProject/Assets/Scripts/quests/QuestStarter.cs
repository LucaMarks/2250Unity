using UnityEngine;

public class QuestStarter : MonoBehaviour
{
    public Quest questToStart;

    public void StartQuest()
    {
        if (QuestManager.Instance == null)
        {
            QuestManager.Instance = FindFirstObjectByType<QuestManager>();
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("No QuestManager found in scene.");
            return;
        }

        Debug.Log("Quest started...");
        QuestManager.Instance.StartQuest(questToStart);
    }
}
