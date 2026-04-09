using UnityEngine;

public class HpObject : MonoBehaviour
{
    public int healthIncrease = 100;

    public void Interact(Player player)
    {
        if (player != null)
        {
            player.Health += healthIncrease;

            Debug.Log("Player gained +" + healthIncrease + " HP!");

            Destroy(gameObject); // remove object after pickup
            Debug.Log("Player Health" + player.Health);
        }
    }
}