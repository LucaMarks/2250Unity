using UnityEngine;

public class MainQuest : Quest
{
    public Player player;
    public Item rudderItem;
    // publid Item

    public void Awake()
    {
        dialogueToUnlock = associatedNPC.dialogueStages[2];
    }

    public override string GetObjectiveText()
    {
        // throw new System.NotImplementedException();
        string builder = "Test Objective";
        return builder;
    }

    public override bool CheckIfComplete()
    {
        if (player.inventory.hasItem(rudderItem))
        {
            // isCompleted = true;
            return true;
        }

        return false;
    }

    protected override void GiveReward()
    {
        base.GiveReward();
        player.inventory.addItem(waterGemstone);        
        Debug.Log("Water Gemstone reward is added to your inventory");
    }
}