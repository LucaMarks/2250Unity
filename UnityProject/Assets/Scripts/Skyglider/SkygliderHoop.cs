using UnityEngine;

public class SkygliderHoop : MonoBehaviour
{
    public SkygliderTrialQuest quest;
    private bool wasCompleted;

    private void OnTriggerEnter(Collider other)
    {
        if (wasCompleted)
            return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null || quest == null)
            return;

        wasCompleted = true;
        quest.RegisterHoopCompleted();
    }
}
