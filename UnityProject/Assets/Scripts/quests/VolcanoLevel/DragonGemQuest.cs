using UnityEngine;

public class DragonGemQuest : Quest
{
    public Player player;

    [Header("Boss Requirement")]
    public KillRedDragon dragonQuest; // reference to your dragon quest

    [Header("Reward")]
    public Item redGemstone;

    public void Awake()
    {
        dialogueToUnlock = associatedNPC.dialogueStages[1];
    }

    public override string GetObjectiveText()
    {
        string builder = "Defeat the Red Dragon and claim the Red Gemstone";
        return builder;
    }

    public override bool CheckIfComplete()
    {
        // This quest completes when the dragon quest is done
        if (dragonQuest != null && dragonQuest.isCompleted)
        {
            return true;
        }

        return false;
    }

    protected override void GiveReward()
    {
        base.GiveReward();

        if (player != null && redGemstone != null)
        {
            player.inventory.addItem(redGemstone);
            Debug.Log("Red Gemstone reward is added to your inventory");
        }
        else
        {
            Debug.LogWarning("Player or Red Gemstone not assigned in DragonGemQuest");
        }
    }
}