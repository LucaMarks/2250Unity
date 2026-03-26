using UnityEngine;

public class RunState : ActorState
{
    public RunState(Actor actor, Animator animator) : base(actor, animator) { }

    public override void Enter()
    {
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        actor.Move(); 

        //if no input, go back to idle
        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            actor.ChangeState(new IdleState(actor, animator));
        }

        //input while running could transition to an attack state (running attack?)
        if (Input.GetButtonDown("Fire1"))
        {
            actor.ChangeState(new AttackState(actor, animator));
        }
    }
}