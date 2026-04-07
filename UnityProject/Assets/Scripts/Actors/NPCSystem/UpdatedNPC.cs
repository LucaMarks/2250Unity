using UnityEngine;

[System.Serializable]
public class DialogueStage
{
    [TextArea(2, 5)]
    public string[] lines;

    public bool isLocked;

    [TextArea(2, 5)]
    public string[] lockedLines;
}

public class UpdatedNPC : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;

    [Header("Dialogue Stages")]
    public DialogueStage[] dialogueStages;

    private int currentStage = 0;

    public void StartDialogue()
    {
        UpdatedDialogueSystem dialogueSystem = FindFirstObjectByType<UpdatedDialogueSystem>();

        if (dialogueSystem == null)
        {
            Debug.LogWarning("No UpdatedDialogueSystem found in scene.");
            return;
        }

        if (dialogueStages == null || dialogueStages.Length == 0)
        {
            Debug.LogWarning(npcName + " has no dialogue stages.");
            return;
        }

        if (currentStage < 0 || currentStage >= dialogueStages.Length)
        {
            Debug.LogWarning(npcName + " has an invalid current stage.");
            return;
        }

        DialogueStage stage = dialogueStages[currentStage];

        if (stage.isLocked)
        {
            if (stage.lockedLines != null && stage.lockedLines.Length > 0)
            {
                dialogueSystem.StartDialogue(npcName, stage.lockedLines);
            }
            else
            {
                Debug.LogWarning(npcName + " has a locked stage with no lockedLines set.");
            }

            return;
        }

        if (stage.lines == null || stage.lines.Length == 0)
        {
            Debug.LogWarning(npcName + " has an empty unlocked dialogue stage.");
            return;
        }

        dialogueSystem.StartDialogue(npcName, stage.lines);
    }

    public void AdvanceDialogueStage()
    {
        if (dialogueStages == null || dialogueStages.Length == 0)
            return;

        if (currentStage < dialogueStages.Length - 1)
        {
            currentStage++;
        }
    }

    public void SetDialogueStage(int stage)
    {
        if (dialogueStages == null)
            return;

        if (stage >= 0 && stage < dialogueStages.Length)
        {
            currentStage = stage;
        }
    }

    public int GetDialogueStage()
    {
        return currentStage;
    }
}
