using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public interface IState
{
    public enum States
    {
        // list of states that all objects inheriting from Actor will have
        Death,
        Run,
        Idle,
        Attack,
        TakeDamage,
        UseSkill,
        Shoot
    } //make separate state classes for individual objects
    String Animation { set; get; }
    

    //to-do: after implementing more features, see what more is needed from this interface
}


