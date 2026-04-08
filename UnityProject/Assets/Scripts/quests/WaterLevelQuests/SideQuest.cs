using UnityEngine;

public class SideQuest : Quest
{

    public Player player;
    // public UpdatedNPC thisNPC;
    public Item fireWood;
    private int fireWoodCount;

    private void Awake()
    {
        dialogueToUnlock = associatedNPC.dialogueStages[1];
    }

    public override string GetObjectiveText()
    {
        return "Collect 3 pieces of fire wood...";
    }

    // public override bool checkIfComplete()
    
    public override bool CheckIfComplete()    
    {

        for (int i = 0; i < player.inventory.getLen(); i++)
        {
            if (player.inventory.getItem(i).ID == fireWood.ID)
            {
                fireWoodCount++;
            }            
        }

        // Debug.Log("Fire wood count -> " + fireWoodCount);
        if (fireWoodCount >= 3)
        {
            return true;
        }

        fireWoodCount = 0;
        return false;
    }

    protected override void GiveReward()
    {
        base.GiveReward();
        player.maxWeaponIndex = player.maxWeaponIndex + 1 < player.swordMaterials.Length ? player.maxWeaponIndex+1 : 0;  
        Debug.Log("New weapon unlocked!");
    }
}