using UnityEngine;

public class experianceSystem
{
    private int currentXP = 0;    
    private int xpToLevelUp = 100;
    private int currentLevel = 0;
    
    //adds xp when called
    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;
        
        CheckLevelUp();
    }

    
    private void CheckLevelUp()
    {
        if (currentXP >= xpToLevelUp)
        {
            currentXP = currentXP - xpToLevelUp;
            currentLevel++;
            
            xpToLevelUp += 50 * currentLevel;
        }
    }
    
    //can be used in future for perks or level multipliers based on the level 
    public int GetLevel()
    {
        return currentLevel;
    }
    



}
