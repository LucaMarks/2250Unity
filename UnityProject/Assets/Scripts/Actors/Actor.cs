using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public List<List<int>> Position = new List<List<int>>();
    public int Speed;
    public int Health;
    // to-do: implement rest of fields needed

    private ActorState currentState;
    private Animator animator;
    public List<Item> Drops; // to-do: create Item script, then import

    public Actor(int speed, int health, List<List<int>> position)
    {
        this.Speed = speed;
        this.Health = health;
        this.Position = position;
    }

    // Update is called once per frame
    void Start()
    {
        animator = GetComponent<Animator>();
        ChangeState(new IdleState(this, animator)); //start idle for now
    }

    void Update()
    {
        Move();
        currentState?.Update(); //update current state
    }
    
    public void ChangeState(ActorState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
    public void Move()
    {
        float ForwardMove = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        float TurnMove = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

        transform.Translate(Vector3.forward * ForwardMove);
        transform.Rotate(Vector3.up * TurnMove);
    }

    //to-do: implement state updating with appropriate animations
    
    //to-do: implement attacking (melee or projectile will depend on class) with animations
    void Attack()
    {
        //potentially make this into a separate script
    }
    
    // to-do:
    // trigger the death screen (UI not implemented yet)
    // freeze (x)
    // fade out?
    // play death animation
    // drop currency? 
    void Die()
    {
        // deathstate now freezes the game, drops and ui trigger will go here later
    }

    
}
