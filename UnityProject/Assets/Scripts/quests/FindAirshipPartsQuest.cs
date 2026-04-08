using UnityEngine;

public class FindAirshipPartsQuest : Quest
{
    [Header("Required Parts")]
    public Player player;
    public Item propellor;
    public Item rudder;
    public Item energyCrystal;

    [Header("Completion Dialogue")]
    [Tooltip("The mechanic NPC dialogue stage to unlock when all parts are brought back.")]
    public int mechanicCompletedDialogueStage = 2;

    [Header("Reward")]
    public Item rewardItem;
    public bool removePartsFromInventory = true;

    private string propellorId;
    private string rudderId;
    private string energyCrystalId;

    private void Awake()
    {
        SetDialogueToUnlockFromMechanic();
        CacheRequiredItemIds();
    }

    public override void StartQuest()
    {
        if (!hasStarted)
        {
            Debug.Log("FindAirshipPartsQuest -> first interaction, starting quest.");
            base.StartQuest();
            return;
        }

        // Talking to the mechanic again acts as the turn-in check.
        Debug.Log("FindAirshipPartsQuest -> turn-in check. Propellor=" + Propellor.WasCollected +
                  ", Rudder=" + SkyRudderItem.WasCollected +
                  ", EnergyCrystal=" + EnergyCrystal.WasCollected +
                  ", currentNPCStage=" + (associatedNPC != null ? associatedNPC.currentStage.ToString() : "no npc"));
        if (!isCompleted && CheckIfComplete())
        {
            Debug.Log("FindAirshipPartsQuest -> CheckIfComplete returned true. Completing quest.");
            CompleteQuest();
        }
        else
        {
            Debug.Log("FindAirshipPartsQuest -> CheckIfComplete returned false.");
        }
    }

    public override void UpdateQuest()
    {
        // This quest should only complete when the player talks to the mechanic,
        // not immediately when the last airship part is picked up.
    }

    public override string GetObjectiveText()
    {
        if (player == null)
            return "Find the 3 missing airship parts and return to the mechanic.";

        int partsFound = CountFoundParts();
        return "Find airship parts: " + partsFound + "/3. Return to the mechanic when you have them all.";
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted || player == null)
        {
            Debug.Log("FindAirshipPartsQuest CheckIfComplete blocked. hasStarted=" + hasStarted +
                      ", isCompleted=" + isCompleted +
                      ", playerAssigned=" + (player != null));
            return false;
        }

        bool complete = Propellor.WasCollected &&
                        SkyRudderItem.WasCollected &&
                        EnergyCrystal.WasCollected;

        Debug.Log("FindAirshipPartsQuest CheckIfComplete -> " + complete +
                  " [Propellor=" + Propellor.WasCollected +
                  ", Rudder=" + SkyRudderItem.WasCollected +
                  ", EnergyCrystal=" + EnergyCrystal.WasCollected + "]");

        return complete;
    }

    public override void CompleteQuest()
    {
        SetDialogueToUnlockFromMechanic();

        if (dialogueToUnlock == null)
        {
            Debug.LogWarning("FindAirshipPartsQuest needs an associatedNPC and mechanicCompletedDialogueStage before it can unlock mechanic dialogue.");
            return;
        }

        dialogueToUnlock.isLocked = false;
        Debug.Log("FindAirshipPartsQuest -> unlocked mechanic dialogue stage " + mechanicCompletedDialogueStage);

        if (associatedNPC != null)
        {
            associatedNPC.currentStage = mechanicCompletedDialogueStage;
            Debug.Log("FindAirshipPartsQuest -> mechanic currentStage set to " + associatedNPC.currentStage);
        }

        base.CompleteQuest();
    }

    protected override void GiveReward()
    {
        if (player == null)
            return;

        if (rewardItem != null)
        {
            player.inventory.addItem(rewardItem);
        }

        Debug.Log("FindAirshipPartsQuest -> reward step reached. The mechanic fixed the airship.");
    }

    private int CountFoundParts()
    {
        int count = 0;

        if (Propellor.WasCollected) count++;
        if (SkyRudderItem.WasCollected) count++;
        if (EnergyCrystal.WasCollected) count++;

        return count;
    }

    private void SetDialogueToUnlockFromMechanic()
    {
        if (associatedNPC == null ||
            associatedNPC.dialogueStages == null ||
            mechanicCompletedDialogueStage < 0 ||
            mechanicCompletedDialogueStage >= associatedNPC.dialogueStages.Length)
        {
            return;
        }

        dialogueToUnlock = associatedNPC.dialogueStages[mechanicCompletedDialogueStage];
    }

    private void CacheRequiredItemIds()
    {
        propellorId = propellor != null ? propellor.ID : propellorId;
        rudderId = rudder != null ? rudder.ID : rudderId;
        energyCrystalId = energyCrystal != null ? energyCrystal.ID : energyCrystalId;
    }
}
