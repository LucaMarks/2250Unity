using UnityEngine;
using TMPro;

public class QuestObjectiveUI : MonoBehaviour
{
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveText;

    private void Start()
    {
        HideObjective();
    }

    public void SetObjectiveText(string text)
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }

        if (objectiveText != null)
        {
            objectiveText.text = text;
        }
    }

    public void HideObjective()
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }

        if (objectiveText != null)
        {
            objectiveText.text = "";
        }
    }
}