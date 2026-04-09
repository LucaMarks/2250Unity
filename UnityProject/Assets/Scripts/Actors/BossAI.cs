using UnityEngine;

public class BossAI : EnemyAI
{
    // private Animator animator;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        base.Update();

        // Stop ALL movement (boss stays still)
        agent.isStopped = true;
    }

    protected override void AttackPlayer()
    {
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                animator.SetTrigger("BasicAttack");
            }
            else
            {
                animator.SetTrigger("ClawAttack");
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            animator.SetTrigger("Die");

            // Wait for animation before destroying
            Invoke(nameof(DestroyEnemy), 3f);
        }
    }

    // OPTIONAL: Call this when fight starts
    public void StartFight()
    {
        animator.SetTrigger("Scream");
    }

    // CALLED FROM ANIMATION EVENT (VERY IMPORTANT)
    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * 2f,
            attackRange,
            whatIsPlayer
        );

        foreach (Collider hit in hits)
        {
            Player p = hit.GetComponent<Player>();
            if (p != null)
            {
                p.Health -= Damage;
                Debug.Log("Boss hit player!");
            }
        }
    }
}