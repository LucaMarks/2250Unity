using NUnit.Framework.Interfaces;
using UnityEngine;

public class IdleState : ActorState
{
    public IdleState(Actor actor, Animator animator) : base(actor, animator) { }

    public override void Enter()
    {
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);
        //probably need to create more flags for other states, to-do
    }

    public override void Update()
    {
        //check for movement input to transition into runstate
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            actor.ChangeState(new RunState(actor, animator));
        }

        //check for attack input to trigger attackstate
        if (Input.GetButtonDown("Fire1"))
        {
            actor.ChangeState(new AttackState(actor, animator));
        }
        
        //to-do: implement more checks for the rest of the states
    }
}