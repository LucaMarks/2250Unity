using UnityEngine;

public class SkygliderBasicRange : MonoBehaviour
{
    public SkygliderBasic skyglider;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player == null || skyglider == null)
            return;

        skyglider.SetNearbyPlayer(player);
        Debug.Log("SkygliderBasicRange -> Player entered range.");
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>() ?? other.GetComponentInParent<Player>();
        if (player == null || skyglider == null)
            return;

        skyglider.ClearNearbyPlayer(player);
        Debug.Log("SkygliderBasicRange -> Player left range.");
    }
}
