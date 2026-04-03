using UnityEngine;

public class UpdatedNPC : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;

    [TextArea(2, 5)]
    public string[] dialogueLines;

    public void StartDialogue()
    {
        UpdatedDialogueSystem dialogueSystem = FindFirstObjectByType<UpdatedDialogueSystem>();

        if (dialogueSystem == null)
        {
            Debug.LogWarning("DialogueSystem not found in scene.");
            return;
        }

        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning(npcName + " has no dialogue.");
            return;
        }

        dialogueSystem.StartDialogue(npcName, dialogueLines);
    }
}