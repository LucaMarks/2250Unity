using UnityEngine;
using UnityEngine.UI;
using TMPro;

//displays health xp and level and maybe currency
//to be attatched to canvas gameobject

public class PlayerHUD : MonoBehaviour
{
    public Player player;
    
    //elements in unity
    public TMP_Text healthText;
    public TMP_Text xpText;
    public TMP_Text levelText;
    public TMP_Text currencyText;
    
    //once per frame
    void Update()
    {
        if (player == null) return;
        
        //update health and currency
        healthText.text = $"Health: {player.Health}";
        currencyText.text = $"Money: {player.Currency}";
        
        //error check for XP system
        if (player.progressionSystem != null)
        {
            xpText.text = $"XP: {player.progressionSystem.currentXP}/{player.progressionSystem.xpToNextLevel}";
            levelText.text = $"Level: {player.progressionSystem.currentLevel}";
        }
    }
}