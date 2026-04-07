using UnityEngine;
using UnityEngine.PlayerLoop;

public class RopeInteractionRange : MonoBehaviour
{
    // public StonePlatform stonePlatform;
    public Rope rope;
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            
            player.setCurrRope(rope);                        
            // stonePlatform.movePlatform(rope.dir);
            // rope.animateDown();
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        Player player =  other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.clearCurrRope(rope);            
        }
    }
}