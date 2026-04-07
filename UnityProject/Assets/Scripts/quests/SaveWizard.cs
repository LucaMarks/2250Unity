using UnityEngine;

public class SaveWizard : Quest
{
    [Header("Clear Area Objective")]
    public Transform npcToProtect;
    public float checkRadius = 15f;
    public LayerMask enemyLayers;
    public string enemyTag = "Enemy";

    [Header("Reward")]
    public int goldReward = 100;

    public override string GetObjectiveText()
    {
        return "Defeat all enemies near the Old Wizard";
    }

    public override bool CheckIfComplete()
    {
        if (!hasStarted || isCompleted)
            return false;

        if (npcToProtect == null)
        {
            Debug.LogWarning(questName + " has no npcToProtect assigned.");
            return false;
        }

        Collider[] nearbyColliders = Physics.OverlapSphere(
            npcToProtect.position,
            checkRadius,
            enemyLayers
        );

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            if (nearbyCollider == null)
                continue;

            if (!string.IsNullOrEmpty(enemyTag) && !nearbyCollider.CompareTag(enemyTag))
                continue;

            return false;
        }

        return true;
    }

    protected override void GiveReward()
    {
        Debug.Log("Reward: Player received " + goldReward + " gold.");
    }

    private void OnDrawGizmosSelected()
    {
        if (npcToProtect == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(npcToProtect.position, checkRadius);
    }
}