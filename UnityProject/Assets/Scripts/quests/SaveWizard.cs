using UnityEngine;

public class SaveWizard : Quest
{
    [Header("Assigned Enemies")]
    public EnemyAI[] enemiesToDefeat;

    [Header("Wizard Dialogue")]
    public int thankYouDialogueStage = 1;

    [Header("Reward")]
    public Player player;
    public WizardBoots bootsReward;
    public int goldReward = 100;

    public override string GetObjectiveText()
    {
        int totalEnemies = enemiesToDefeat != null ? enemiesToDefeat.Length : 0;
        int defeatedEnemies = CountDefeatedEnemies();
        return "Defeat the enemies attacking the Old Wizard (" + defeatedEnemies + "/" + totalEnemies + ")";
    }

    public override void StartQuest()
    {
        Debug.Log("SaveWizard -> StartQuest called. hasStarted=" + hasStarted + ", isCompleted=" + isCompleted);
        base.StartQuest();
    }

    public override void UpdateQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        Debug.Log("SaveWizard -> UpdateQuest. Defeated=" + CountDefeatedEnemies() + "/" +
                  (enemiesToDefeat != null ? enemiesToDefeat.Length : 0));

        base.UpdateQuest();
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted)
            return false;

        if (enemiesToDefeat == null || enemiesToDefeat.Length == 0)
        {
            Debug.LogWarning(questName + " has no enemies assigned.");
            return false;
        }

        for (int i = 0; i < enemiesToDefeat.Length; i++)
        {
            if (enemiesToDefeat[i] != null)
            {
                Debug.Log("SaveWizard -> Enemy still alive at index " + i + ": " + enemiesToDefeat[i].name);
                return false;
            }
        }

        Debug.Log("SaveWizard -> All assigned enemies are gone.");
        return true;
    }

    public override void CompleteQuest()
    {
        Debug.Log("SaveWizard -> CompleteQuest called.");

        if (associatedNPC != null &&
            associatedNPC.dialogueStages != null &&
            thankYouDialogueStage >= 0 &&
            thankYouDialogueStage < associatedNPC.dialogueStages.Length)
        {
            associatedNPC.dialogueStages[thankYouDialogueStage].isLocked = false;
            associatedNPC.currentStage = thankYouDialogueStage;
            Debug.Log("SaveWizard -> Unlocked wizard dialogue stage " + thankYouDialogueStage);
        }

        base.CompleteQuest();
    }

    protected override void GiveReward()
    {
        if (player != null)
        {
            player.canDoubleJump = true;

            if (bootsReward != null && !player.inventory.hasItem(bootsReward))
            {
                player.inventory.addItem(bootsReward);
            }
        }

        Debug.Log("Reward: Player received " + goldReward + " gold.");
    }

    private int CountDefeatedEnemies()
    {
        if (enemiesToDefeat == null || enemiesToDefeat.Length == 0)
            return 0;

        int defeatedEnemies = 0;

        for (int i = 0; i < enemiesToDefeat.Length; i++)
        {
            if (enemiesToDefeat[i] == null)
            {
                defeatedEnemies++;
            }
        }

        return defeatedEnemies;
    }
}
