using UnityEngine;

public class SkygliderInteractionRangeV2 : MonoBehaviour
{
    public SkygliderV2 skyglider;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null || skyglider == null)
            return;

        skyglider.SetNearbyPlayer(player);
        Debug.Log("SkygliderInteractionRangeV2 -> Player entered range.");
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null || skyglider == null)
            return;

        skyglider.ClearNearbyPlayer(player);
        Debug.Log("SkygliderInteractionRangeV2 -> Player left range.");
    }
}
