using UnityEngine;

public class DeathState : ActorState
{
    public DeathState(Actor actor, Animator animator) : base(actor, animator) { }

    public override void Enter()
    {
        animator.SetBool("IsDead", true);
        actor.enabled = false; //or freeze input?
        Time.timeScale = 0f; //just generally freezing for now, UI to be implemented later  
    }
}