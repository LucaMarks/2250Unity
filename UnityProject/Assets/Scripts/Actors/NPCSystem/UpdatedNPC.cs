using UnityEngine;

[System.Serializable]
public class DialogueStage
{
    [TextArea(2, 5)]
    public string[] lines;

    public bool isLocked;
    [Header("Start the quest associated with this npc when the player reads this dialogue")] 
    public bool startQuest;

    [TextArea(2, 5)]
    public string[] lockedLines;
}

public class UpdatedNPC : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;

    public QuestStarter[] questStarters;
    private int questStarterIndex = 0;
    
    [Header("Dialogue Stages")]
    public DialogueStage[] dialogueStages;
    public int currentStage = 0;


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
        Debug.Log(npcName + " StartDialogue -> currentStage=" + currentStage +
                  ", isLocked=" + stage.isLocked +
                  ", startQuest=" + stage.startQuest);

        if (stage.startQuest)
        {
            if (questStarters == null || questStarters.Length == 0)
            {
                Debug.LogWarning(npcName + " needs at least one QuestStarter assigned.");
            }
            else
            {
                int safeQuestStarterIndex = Mathf.Clamp(questStarterIndex, 0, questStarters.Length - 1);
                QuestStarter questStarter = questStarters[safeQuestStarterIndex];

                if (questStarter != null)
                {
                    Debug.Log(npcName + " calling QuestStarter at index " + safeQuestStarterIndex);
                    questStarter.StartQuest();

                    // Reuse the last quest starter instead of running off the end of the array.
                    if (questStarterIndex < questStarters.Length - 1)
                    {
                        questStarterIndex++;
                    }
                }
                else
                {
                    Debug.LogWarning("Add quest starter element to QuestStarters list in UpdatedNPC inspector.");
                }
            }

            // Re-read the current stage after quest logic runs because the quest
            // may have unlocked or jumped the NPC to a completion stage.
            if (currentStage >= 0 && currentStage < dialogueStages.Length)
            {
                stage = dialogueStages[currentStage];
                Debug.Log(npcName + " refreshed dialogue stage after quest logic -> currentStage=" + currentStage +
                          ", isLocked=" + stage.isLocked +
                          ", startQuest=" + stage.startQuest);
            }
        }

        if (stage.isLocked)
        {
            if (stage.lockedLines != null && stage.lockedLines.Length > 0)
            {
                dialogueSystem.StartDialogue(this, npcName, stage.lockedLines);
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

        dialogueSystem.StartDialogue(this, npcName, stage.lines);

        if (ShouldAdvanceToNextStage())
        {
            Debug.Log(npcName + " advancing dialogue stage from " + currentStage + " to " + (currentStage + 1));
            currentStage++;
        }
        else
        {
            Debug.Log(npcName + " staying on dialogue stage " + currentStage);
        }
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

    private bool ShouldAdvanceToNextStage()
    {
        int nextStageIndex = currentStage + 1;

        if (nextStageIndex >= dialogueStages.Length)
        {
            Debug.Log(npcName + " has no next dialogue stage to advance to.");
            return false;
        }

        // Quest-giver NPCs often need to stay on an in-progress stage until the
        // completion stage unlocks.
        if (dialogueStages[nextStageIndex].isLocked)
        {
            Debug.Log(npcName + " next dialogue stage " + nextStageIndex + " is locked, so current stage repeats.");
            return false;
        }

        return true;
    }
}
