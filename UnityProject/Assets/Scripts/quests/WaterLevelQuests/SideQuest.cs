public class SideQuest : Quest
{

    private Item[] collectedFireWood = new Item[3];

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
        int count = 0;
        for (int i = 0; i < collectedFireWood.Length; i++)
        {
            if (collectedFireWood[i] == fireWood)
            {
                count++;
            }
        }
        return count == 3;
    }

}