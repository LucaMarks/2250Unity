using UnityEngine;

public class AttackState : ActorState
{
    private float attackDuration = 0.5f; //set a duration for the attack animation, test number currently implemented
    private float timer;

    public AttackState(Actor actor, Animator animator) : base(actor, animator) { }

    public override void Enter()
    {
        animator.SetTrigger("Attack");  // use a trigger for one-shot attacks
        timer = attackDuration;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
                actor.ChangeState(new RunState(actor, animator));
            else
                actor.ChangeState(new IdleState(actor, animator));
        }
    }

    public override void Exit()
    {
        //reset flags?
    }
}