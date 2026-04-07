using UnityEngine;

// Attach to the Earth Golem boss GameObject in EarthLevel3
// instead of plain EnemyAI - this gives it higher HP and a stomp attack
// Set up the same NavMeshAgent, sight/attack range, and whatIsPlayer layer in the Inspector
public class EarthGolem : EnemyAI
{
    [Header("Golem Settings")]
    public int golemHealth = 300;
    public float stompRadius = 3f;
    public int stompDamage = 30;

    private bool stompOnCooldown = false;

    private void Start()
    {
        // EnemyAI's private Awake already ran, now we override the stats
        Health = golemHealth;
        Damage = 20;
    }

    // override the attack so the golem does a ground stomp instead of a poke
    protected override void AttackPlayer()
    {
        if (stompOnCooldown) return;

        // stomp - damages everything in a radius around the golem
        Collider[] hits = Physics.OverlapSphere(transform.position, stompRadius);
        foreach (Collider hit in hits)
        {
            Player player = hit.GetComponent<Player>();
            if (player == null)
                player = hit.GetComponentInParent<Player>();

            if (player != null)
            {
                player.Health -= stompDamage;
                Debug.Log("Earth Golem stomp hit player for " + stompDamage);
            }
        }

        stompOnCooldown = true;
        Invoke(nameof(ResetStomp), timeBetweenAttacks);
    }

    private void ResetStomp()
    {
        stompOnCooldown = false;
    }

    // when the golem dies, log it (you can hook up any win condition here)
    protected override void DestroyEnemy()
    {
        Debug.Log("Earth Golem defeated! Level complete.");
        base.DestroyEnemy();
    }

    // show stomp radius as a gizmo in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stompRadius);
    }
}
