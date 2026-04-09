using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Actor : MonoBehaviour
{
    public List<List<int>> Position = new List<List<int>>();//i don't think we actually need this


    public GameObject thisObject;
    


    public int Speed;
    public int Health;
    public int Damage;
    // to-do: implement rest of fields needed

    private ActorState currentState;
    protected Animator animator;
    public List<Item> Drops; // to-do: create Item script, then import

    //this is 
    public List<string> Dialogue;
    private int DialogueIndex = 0;

    public int attackCooldown;

    public Actor()
    {
    }

    public Actor(int speed, int health, int damage)
    {
        this.Speed = speed;
        this.Health = health;
        this.Damage = damage;
    }

    public Actor(int speed, int health)
    {
        this.Speed = speed;
        this.Health = health;
    }

    // Update is called once per frame
    private void Start()
    {
        // animator = GetComponent<Animator>();
        // ChangeState(new IdleState(this, animator)); //start idle for now -> Commented this out temp -Luca


        // Debug.Log("Start is called for actor");
    }

    public virtual void Update()
    {
        Move();
        // currentState?.Update(); //update current state -> commented out for now -Luca
    }
    
    public void ChangeState(ActorState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
    public virtual void Move()
    {

        // Debug.Log(moveAction.ReadValue<Vector2>());


        

        //legacy input system
        // float ForwardMove = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        // float TurnMove = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        //
        // transform.Translate(Vector3.forward * ForwardMove);
        // transform.Rotate(Vector3.up * TurnMove);
        //
        // //get mouse input
        // float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * 400; //400 -> mouse sensitivity
        // float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * 400;
        //
        // yRotation += mouseX;
        //
        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //
        // //rotate cam & orientation
        // transform.rotation = Quaternion.Euler(xRotation, yRotation, 0); 
        // orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        
    }

    //to-do: implement state updating with appropriate animations
    
    //to-do: implement attacking (melee or projectile will depend on class) with animations
    public virtual void Attack()
    {
        //potentially make this into a separate script
    }
    
    // to-do:
    // trigger the death screen (UI not implemented yet)
    // freeze (x)
    // fade out?
    // play death animation
    // drop currency? 
    protected virtual void Die()
    {
        Debug.Log("Actor died");
    }

    //this method isn't used in the current npc system
    public void OutputDialogue()
    {
        //draw dialouge to screen
        //Dialogue[DialogueIndex];
        DialogueIndex++;
        if (DialogueIndex >= Dialogue.Count){DialogueIndex = 0;}
    }

    
}
