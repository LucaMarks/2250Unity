using UnityEngine;

//Main quest for the Earth level
public class EarthMainQuest : Quest
{
    [Header("Earth Quest")]
    public Player player;

    public override string GetObjectiveText()
    {
        //puzzle and boss both need to be done
        if (!EarthGolem.isDefeated && Pillar.activatedCount < Pillar.totalPillars)
            return "Solve the light beam puzzle and defeat the Earth Golem.";
        if (!EarthGolem.isDefeated)
            return "Puzzle solved! Now defeat the Earth Golem.";
        if (Pillar.activatedCount < Pillar.totalPillars)
            return "Golem defeated! Now solve the light beam puzzle.";
        return "Both conditions met — find the exit!";
    }

    public override bool CheckIfComplete()
    {
        return EarthGolem.isDefeated && Pillar.activatedCount >= Pillar.totalPillars;
    }

    //override CompleteQuest to skip the dialogueToUnlock line (not using)
    public override void CompleteQuest()
    {
        if (!hasStarted || isCompleted) return;
        if (!CheckIfComplete()) return;

        isCompleted = true;
        GiveReward();
        Debug.Log("Earth Main Quest complete!");
    }

    protected override void GiveReward()
    {
        base.GiveReward();
        if (player != null)
        {
            player.inventory.addItem(earthGemstone);
            Debug.Log("Earth Gemstone added to inventory!");
        }
        else
        {
            Debug.LogWarning("EarthMainQuest: player reference not set in Inspector.");
        }
    }
}
