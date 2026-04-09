using UnityEngine;

public class KillRedDragon : Quest
{
    [Header("Reward")]
    public int goldReward = 200;

    private bool dragonKilled = false;

    public override string GetObjectiveText()
    {
        return "Defeat the Red Dragon";
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted)
            return false;

        return dragonKilled;
    }

    public void OnDragonKilled()
    {
        dragonKilled = true;
    }

    protected override void GiveReward()
    {
        Debug.Log("Reward: Player received " + goldReward + " gold.");
    }
}