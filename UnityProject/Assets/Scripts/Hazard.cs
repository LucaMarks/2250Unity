using UnityEngine;

// Attach to any hazard object (stalactite, lava, spike, etc.)
// Add a Collider to the GameObject and tick "Is Trigger"
// Set the Damage value in the Inspector
// Player loses HP on contact, with a cooldown so they don't get hit every frame
public class Hazard : StaticObject
{
    [Header("Hazard Settings")]
    public int Damage = 10;
    public float damageCooldown = 1f; // seconds between each damage tick

    private float lastDamageTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // keep dealing damage while player stays inside (e.g. lava)
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider other)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;

        Player player = other.GetComponent<Player>();
        if (player == null)
            player = other.GetComponentInParent<Player>();

        if (player != null)
        {
            player.Health -= Damage;
            lastDamageTime = Time.time;
            Debug.Log("Hazard hit player for " + Damage + " damage. Player HP: " + player.Health);
        }
    }
}
