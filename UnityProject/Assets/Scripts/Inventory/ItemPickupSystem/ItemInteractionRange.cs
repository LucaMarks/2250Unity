using UnityEngine;

public class ItemInteractionRange : MonoBehaviour
{

    public Item item;
    // private bool playerInRange = false;
    
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            // playerInRange = true;                        
            player.setCurrItem(item);
        }else{Debug.Log("Cannot find player for item Pickup system (ItenInteractionRange.cs)");}
    }

    private void OnTriggerExit(Collider other)
    {
        
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            // playerInRange = false;
            player.clearCurrItem(item);
        }else{Debug.Log("Cannot find player for item Pickup system (ItemInteractionRange.cs)");}
    }        
}