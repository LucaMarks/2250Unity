using UnityEngine;

public class InstantKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player == null)
            return;

        player.Health = 0;
        Debug.Log("InstantKillTrigger -> player killed by touching " + name);
    }
}
