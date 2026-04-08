using UnityEngine;

public class InRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            // Debug.Log("InRange");
            player.setIsInRangeOfShip(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.setIsInRangeOfShip(false);
        }
    }
}