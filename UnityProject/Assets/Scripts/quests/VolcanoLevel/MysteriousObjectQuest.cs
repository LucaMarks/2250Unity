using UnityEngine;

public class MysteriousObjectQuest : Quest
{
    public Player player;

    [Header("Objective")]
    public Item mysteriousObject;   // the item player must pick up

    [Header("Reward")]
    public int goldReward = 150;

    public void Awake()
    {
        dialogueToUnlock = associatedNPC.dialogueStages[1];
    }

    public override string GetObjectiveText()
    {
        string builder = "Retrieve the mysterious object hidden in the ruins";
        return builder;
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted)
            return false;

        if (player != null && mysteriousObject != null)
        {
            if (player.inventory.hasItem(mysteriousObject))
            {
                return true;
            }
        }

        return false;
    }

    protected override void GiveReward()
    {
        base.GiveReward();

        if (player != null)
        {
            player.Currency += goldReward; // assuming you have gold on player
            Debug.Log("Player received " + goldReward + " gold for retrieving the object");
        }
        else
        {
            Debug.LogWarning("Player not assigned in RetrieveMysteriousObject quest");
        }
    }
}
