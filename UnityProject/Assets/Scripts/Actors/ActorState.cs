using UnityEngine;

public abstract class ActorState
{
    protected Actor actor;
    protected Animator animator;

    public ActorState(Actor actor, Animator animator)
    {
        this.actor = actor;
        this.animator = animator;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    //more methods? TBD until more code is written
}