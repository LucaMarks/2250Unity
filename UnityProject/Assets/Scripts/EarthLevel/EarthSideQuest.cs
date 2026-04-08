using UnityEngine;

//side quest for EarthLevel2 escort the Earth Spirit through the hard path
//quest starts when player talks to the Earth Spirit NPC
public class EarthSideQuest : Quest
{
    [Header("Earth Side Quest")]
    public Player player;

    //set this to true when the Earth Spirit reaches the end of the hard path
    public static bool spiritReachedGoal = false;

    private void OnEnable()
    {
        //reset when scene loads
        spiritReachedGoal = false;
    }

    public override string GetObjectiveText()
    {
        if (!spiritReachedGoal)
            return "Escort the Earth Spirit safely through the hard path.";
        return "Earth Spirit is safe! Return to the exit.";
    }

    public override bool CheckIfComplete()
    {
        return spiritReachedGoal;
    }

    //override CompleteQuest to skip the old dialogueToUnlock system
    public override void CompleteQuest()
    {
        if (!hasStarted || isCompleted) return;
        if (!CheckIfComplete()) return;

        isCompleted = true;
        GiveReward();
        Debug.Log("Earth Side Quest complete!");
    }

    protected override void GiveReward()
    {
        base.GiveReward();
        if (player != null)
        {
            //reward XP
            ProgressionSystem progression = player.GetComponent<ProgressionSystem>();
            if (progression != null)
            {
                progression.AwardXP(150, "Earth Spirit escort quest");
                Debug.Log("Side quest reward: 150 XP added.");
            }
            else
            {
                Debug.LogWarning("no ProgressionSystem found on player.");
            }
        }
        else
        {
            Debug.LogWarning("player reference not set in Inspector.");
        }
    }
}
