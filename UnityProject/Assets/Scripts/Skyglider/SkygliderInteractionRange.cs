using UnityEngine;

public class SkygliderInteractionRange : MonoBehaviour
{
    public Skyglider skyglider;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            Debug.Log("SkygliderInteractionRange -> player entered trigger: " + player.name);
            skyglider.SetNearbyPlayer(player);
            Debug.Log("Player entered skyglider range.");
        }
        else
        {
            Debug.Log("Cannot find player for skyglider range.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            Debug.Log("SkygliderInteractionRange -> player left trigger: " + player.name);
            skyglider.ClearNearbyPlayer(player);
            Debug.Log("Player left skyglider range.");
        }
    }
}
