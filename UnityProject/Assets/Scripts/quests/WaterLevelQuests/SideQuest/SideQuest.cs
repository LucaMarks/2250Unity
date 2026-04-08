public class SideQuest : Quest
{

    public Item fireWood;
    public Player player;
    public Item[] collectedFireWood;
    private static int totalFireWood = 0;
    private void Awake()
    {
        fireWood = new Item("FireWood", "Fire Wood", 5);
    }

    public override string GetObjectiveText()
    {
        string builder = "";
        return builder;
        
    }

    public override bool CheckIfComplete()
    {
        // totalFireWood++;
        //         
        // return totalFireWood >= 3;
        return player.inventory.hasItem(fireWood);
    }

    protected override void GiveReward()
    {
        base.GiveReward();
        //this works in our specific use case even though this object fireWood & the FireWood item in the player's inventory completely different objects
        player.inventory.removeItem(fireWood);
    }
}