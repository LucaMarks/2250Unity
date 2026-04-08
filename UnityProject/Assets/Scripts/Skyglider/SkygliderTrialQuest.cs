using UnityEngine;

public class SkygliderTrialQuest : Quest
{
    [Header("Trial Progress")]
    public int totalHoops = 0;
    public int hoopsCompleted = 0;
    public bool reachedFinishIsland = false;

    [Header("Completion Dialogue")]
    [Tooltip("The NPC dialogue stage to unlock after the skyglider trial is finished.")]
    public int completedDialogueStage = 2;

    [Header("Reward")]
    public Item rewardItem;

    private void Awake()
    {
        SetDialogueToUnlock();
    }

    public override void StartQuest()
    {
        if (!hasStarted)
        {
            Debug.Log("SkygliderTrialQuest -> first interaction, starting quest.");
            base.StartQuest();
            return;
        }

        Debug.Log("SkygliderTrialQuest -> turn-in check. hoopsCompleted=" + hoopsCompleted +
                  "/" + totalHoops + ", reachedFinishIsland=" + reachedFinishIsland);

        if (!isCompleted && CheckIfComplete())
        {
            Debug.Log("SkygliderTrialQuest -> completion conditions met.");
            CompleteQuest();
        }
    }

    public override void UpdateQuest()
    {
        // This quest should complete only when the player returns and talks
        // to the NPC after finishing the trial.
    }

    public override string GetObjectiveText()
    {
        return "Fly through hoops: " + hoopsCompleted + "/" + totalHoops + " and reach the distant island.";
    }

    public override bool CheckIfComplete()
    {
        bool complete = hasStarted &&
                        !isCompleted &&
                        hoopsCompleted >= totalHoops &&
                        reachedFinishIsland;

        Debug.Log("SkygliderTrialQuest CheckIfComplete -> " + complete +
                  " [hasStarted=" + hasStarted +
                  ", hoopsCompleted=" + hoopsCompleted +
                  ", totalHoops=" + totalHoops +
                  ", reachedFinishIsland=" + reachedFinishIsland + "]");

        return complete;
    }

    public override void CompleteQuest()
    {
        SetDialogueToUnlock();

        if (dialogueToUnlock == null)
        {
            Debug.LogWarning("SkygliderTrialQuest needs an associatedNPC and completedDialogueStage before it can unlock dialogue.");
            return;
        }

        dialogueToUnlock.isLocked = false;
        Debug.Log("SkygliderTrialQuest -> unlocked NPC dialogue stage " + completedDialogueStage);

        if (associatedNPC != null)
        {
            associatedNPC.currentStage = completedDialogueStage;
            Debug.Log("SkygliderTrialQuest -> NPC currentStage set to " + associatedNPC.currentStage);
        }

        base.CompleteQuest();
    }

    protected override void GiveReward()
    {
        Player player = FindFirstObjectByType<Player>();

        if (rewardItem != null && player != null)
        {
            player.inventory.addItem(rewardItem);
        }

        Debug.Log("SkygliderTrialQuest -> reward step reached.");
    }

    public void RegisterHoopCompleted()
    {
        if (!hasStarted || isCompleted)
            return;

        hoopsCompleted++;
        Debug.Log("SkygliderTrialQuest -> hoop completed. Progress: " + hoopsCompleted + "/" + totalHoops);
    }

    public void RegisterFinishIslandReached()
    {
        if (!hasStarted || isCompleted)
            return;

        reachedFinishIsland = true;
        Debug.Log("SkygliderTrialQuest -> distant island reached.");
    }

    private void SetDialogueToUnlock()
    {
        if (associatedNPC == null ||
            associatedNPC.dialogueStages == null ||
            completedDialogueStage < 0 ||
            completedDialogueStage >= associatedNPC.dialogueStages.Length)
        {
            return;
        }

        dialogueToUnlock = associatedNPC.dialogueStages[completedDialogueStage];
    }
}
