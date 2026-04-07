using UnityEngine;

public class QuestStarter : MonoBehaviour
{
    public Quest questToStart;

    public void StartQuest()
    {
        if (QuestManager.Instance == null)
        {
            QuestManager.Instance = new QuestManager();
            // Debug.LogWarning("No QuestManager found in scene.");
            // return;
        }

        Debug.Log("Quest started...");
        QuestManager.Instance.StartQuest(questToStart);
    }
}