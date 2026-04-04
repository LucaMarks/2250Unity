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
    [Header("NPC Info")] public string npcName;


    [Header("Dialogue Stages")] public DialogueStage[] dialogueStages;

    private int currentStage = 0;

    public void StartDialogue()
    {
        UpdatedDialogueSystem dialogueSystem = FindFirstObjectByType<UpdatedDialogueSystem>();

        if (dialogueStages == null || dialogueStages.Length == 0)
        {
            Debug.LogWarning(npcName + " has no dialogue stages.");
            return;
        }

        if (dialogueStages[currentStage].lines == null || dialogueStages[currentStage].lines.Length == 0)
        {
            Debug.LogWarning(npcName + " has an empty dialogue stage.");
            return;
        }

        DialogueStage stage = dialogueStages[currentStage];

        // Stage 0 is never treated as locked
        if (currentStage == 0)
        {
            if (stage.lines == null || stage.lines.Length == 0)
            {
                Debug.LogWarning(npcName + " stage 0 has no dialogue.");
                return;
            }

            if (currentStage < dialogueStages.Length - 1)
            {
                currentStage++;
            }
            dialogueSystem.StartDialogue(npcName, stage.lines);
            return;
        }


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
        if (currentStage < dialogueStages.Length - 1)
        {
            currentStage++;
        }
    }

    public void SetDialogueStage(int stage)
    {
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
