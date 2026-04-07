using UnityEngine;

public class QuestStarter : MonoBehaviour
{
    public Quest questToStart;

    public void StartQuest()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("No QuestManager found in scene.");
            return;
        }

        QuestManager.Instance.StartQuest(questToStart);
    }
}