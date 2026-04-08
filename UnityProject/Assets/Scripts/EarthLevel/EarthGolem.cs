using UnityEngine;


//instead of plain EnemyAI - this gives it higher HP and a stomp attack
//set up the same NavMeshAgent, sight/attack range, and whatIsPlayer layer in the Inspector
public class EarthGolem : EnemyAI
{
    [Header("Golem Settings")]
    public int golemHealth = 300;
    public float stompRadius = 3f;
    public int stompDamage = 30;

    private bool stompOnCooldown = false;

    //static bool so SceneTransition can check if golem is dead
    public static bool isDefeated = false;

    private void OnEnable()
    {
        //reset when golem spawns (handles scene reload) for example when you die
        isDefeated = false;
    }

    private void Start()
    {
        //EnemyAI's private Awake already ran, now we override the stats
        Health = golemHealth;
        Damage = 20;
    }

    //override the attack so the golem does a ground stomp instead of a poke
    protected override void AttackPlayer()
    {
        if (stompOnCooldown) return;

        //stomp - damages everything in a radius around the golem
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

    protected override void DestroyEnemy()
    {
        isDefeated = true;//update so that the player can go on
        Debug.Log("Earth Golem defeated!");
        base.DestroyEnemy();
    }

    //show stomp radius as a gizmo in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stompRadius);
    }
}
