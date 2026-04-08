using UnityEngine;

public class SkygliderFinishZone : MonoBehaviour
{
    public SkygliderTrialQuest quest;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null || quest == null)
            return;

        quest.RegisterFinishIslandReached();
    }
}
