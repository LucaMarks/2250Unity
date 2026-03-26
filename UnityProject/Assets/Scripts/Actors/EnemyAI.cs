using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Actor 
{
    private Renderer enemyRenderer;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    // public float health; we don't need this since it is in the actor class
    public Player playerComponents;
    
    //Patrolling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    
    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectile;
    // private int attackCooldown;//moved this to actor class
    
    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Knight").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        //these are variables from actor
        Health = 100;
        Damage = 25;

    }

    private void Update()
    {
        base.Update();
        attackCooldown++;
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        // Debug.Log("Sight: " + playerInSightRange + " Attack: " + playerInAttackRange);
        
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            enemyRenderer.material.color = Color.green;
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            enemyRenderer.material.color = Color.yellow;
        }
        else if (playerInAttackRange)
        {
            enemyRenderer.material.color = Color.red;
            if (attackCooldown > 60)
            {
                attackCooldown = 0;
                AttackPlayer();
            }
        }
    }

    private void Patroling()
    {
   if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    protected virtual void AttackPlayer()
    {
        playerComponents.Health -= Damage;
        Debug.Log("Melee hit! by " + gameObject.name);
        // Debug.Log("Player health is " +  playerComponents.Health);

        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Point slightly in front of enemy
            Vector3 attackPoint = transform.position + transform.forward * 0.8f;

            // Small melee range (you can keep 1!)
            float radius = attackRange;

            Collider[] hits = Physics.OverlapSphere(attackPoint, radius, whatIsPlayer);

            foreach (Collider hit in hits)
            {
                Player playerScript = hit.GetComponent<Player>();
                if (playerScript != null)
                {
                   //Add player take damge method

                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    protected void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Debug.Log(this.name + " Killed");
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
