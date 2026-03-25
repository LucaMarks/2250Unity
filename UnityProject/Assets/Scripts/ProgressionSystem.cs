using UnityEngine;

//ensure when this code is linked to the gameobject with the player script attatched as well!
//tracks and awards xp changing play attributes, health and damage
//xp can be awarded from quests, combat or item collection, we could maybe add one specifically for side quests.
public class ProgressionSystem : MonoBehaviour
{
    public int currentXP = 0;
    public int currentLevel = 1;
    public int xpToNextLevel = 100;
    
    //method for whenever XP is supposed to be gained
    public void AwardXP(int amount, string source)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP from {source}. Total: {currentXP}/{xpToNextLevel}");
        CheckLevelUp();
    }
    
    //method to validate if a level up has happened
    private void CheckLevelUp()
    {
        if (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.3f);//each level requires 30% more xp than the last
            
            OnLevelUp();
        }
    }

    private void OnLevelUp()
    {
        Debug.Log($"Leveled up! Now level {currentLevel}");
        //simple upgrade stats behaviour
        Player player = GetComponent<Player>();
        if (player != null)
        {
            player.Health += 20;
            player.Damage += 2;
        }
    }
    
    //3 different XP sources

    public void AwardCombatXP()
    {
        AwardXP(30, "combat");
    }
    
    public void AwardQuestXP()
    {
        AwardXP(50, "quest completion");
    }

    public void AwardItemXP()
    {
        AwardXP(10, "item collection");
    }
}