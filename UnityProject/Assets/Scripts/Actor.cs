using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Actor : MonoBehaviour //potentially change this to an interface?
{
    public float Speed;
    public int Health;
    public enum Actor.State; //make separate state classes for individual objects
    public List<Item> Items; // to-do: create Item script, then import
    
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float ForwardMove =  Input.GetAxis("Vertical") * Speed * Time.deltaTime;
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
        Time.timeScale = 0f;
    }
}
