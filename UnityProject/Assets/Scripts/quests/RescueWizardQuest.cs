using UnityEngine;

public class RescueWizardQuest : Quest
{
    [Header("Rescue Setup")]
    public Player player;
    public UpdatedNPC rescuedWizardNPC;
    public int wizardThankYouStage = 1;
    public EnemyAI[] enemiesToDefeat = new EnemyAI[4];

    [Header("Reward")]
    public WizardBoots bootsReward;

    private void Awake()
    {
        SetWizardDialogueToUnlock();
    }

    public override void StartQuest()
    {
        Debug.Log("RescueWizardQuest -> StartQuest called. hasStarted=" + hasStarted + ", isCompleted=" + isCompleted);
        base.StartQuest();
    }

    public override void UpdateQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        Debug.Log("RescueWizardQuest -> UpdateQuest check. Defeated=" + CountDefeatedEnemies() + "/" +
                  (enemiesToDefeat != null ? enemiesToDefeat.Length : 0));

        if (CheckIfComplete())
        {
            Debug.Log("RescueWizardQuest -> All enemies defeated. Completing quest.");
            CompleteQuest();
        }
    }

    public override string GetObjectiveText()
    {
        int totalEnemies = enemiesToDefeat != null ? enemiesToDefeat.Length : 0;
        int defeatedEnemies = CountDefeatedEnemies();

        if (CheckIfEnemiesCleared())
        {
            return "The wizard is safe.";
        }

        return "Save the wizard: defeat the enemies on the island (" + defeatedEnemies + "/" + totalEnemies + ")";
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted)
            return false;

        return CheckIfEnemiesCleared();
    }

    public override void CompleteQuest()
    {
        SetWizardDialogueToUnlock();

        Debug.Log("RescueWizardQuest -> CompleteQuest called. dialogueToUnlock=" +
                  (dialogueToUnlock != null ? "assigned" : "null") +
                  ", wizardThankYouStage=" + wizardThankYouStage);

        if (dialogueToUnlock != null)
        {
            dialogueToUnlock.isLocked = false;
            Debug.Log("RescueWizardQuest -> Wizard thank-you dialogue unlocked.");
        }

        if (rescuedWizardNPC != null)
        {
            rescuedWizardNPC.currentStage = wizardThankYouStage;
            Debug.Log("RescueWizardQuest -> Wizard current stage set to " + rescuedWizardNPC.currentStage);
        }

        base.CompleteQuest();
    }

    protected override void GiveReward()
    {
        if (player != null)
        {
            player.canDoubleJump = true;

            if (bootsReward != null)
            {
                player.inventory.addItem(bootsReward);
            }
        }

        Debug.Log("RescueWizardQuest -> Wizard rescued. Double jump unlocked.");
    }

    private bool CheckIfEnemiesCleared()
    {
        if (enemiesToDefeat == null || enemiesToDefeat.Length == 0)
        {
            Debug.Log("RescueWizardQuest -> No enemies assigned.");
            return false;
        }

        for (int i = 0; i < enemiesToDefeat.Length; i++)
        {
            if (enemiesToDefeat[i] != null)
            {
                Debug.Log("RescueWizardQuest -> Enemy still alive at index " + i + ": " + enemiesToDefeat[i].name);
                return false;
            }
        }

        return true;
    }

    private int CountDefeatedEnemies()
    {
        if (enemiesToDefeat == null || enemiesToDefeat.Length == 0)
            return 0;

        int defeatedCount = 0;
        for (int i = 0; i < enemiesToDefeat.Length; i++)
        {
            if (enemiesToDefeat[i] == null)
            {
                defeatedCount++;
            }
        }

        return defeatedCount;
    }

    private void SetWizardDialogueToUnlock()
    {
        if (rescuedWizardNPC == null ||
            rescuedWizardNPC.dialogueStages == null ||
            wizardThankYouStage < 0 ||
            wizardThankYouStage >= rescuedWizardNPC.dialogueStages.Length)
        {
            return;
        }

        associatedNPC = rescuedWizardNPC;
        dialogueToUnlock = rescuedWizardNPC.dialogueStages[wizardThankYouStage];
    }
}
