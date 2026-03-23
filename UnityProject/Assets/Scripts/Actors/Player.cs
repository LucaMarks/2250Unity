using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor //this also gives us access to MonoBehavoiour
{
    public int Currency = 100;//default currency
    private List<string> skills;
    public Inventory inventory = new Inventory();
    private int SkillPoints;

    private Transform cameraPivot;
    private float lookSensitivity = 0.5f;
    private float pitch;
    private float yaw;


    // public int speed;

    // public int health;

    // public int damage;

    // private float xRotation;

    // private float yRotation;
    // public Player(int speed, int health, int damage, float xRotation , float yRotation) : base(health, damage, xRotation, yRotation) //i don't think this gets called when the game starts
    // {
    //     // Debug.Log("Player created");
    //     //
    // }

    private void Start()
    {
        Debug.Log("Player created");
        // base(health, damage, xRotation, yRotation);
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");

        playerOrientation = GetComponent<PlayerInput>();
        orientationAction = playerOrientation.actions.FindAction("Orientation");
        if (cameraPivot == null)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraPivot = cam.transform;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }    

    private void HazardCollide(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent<Hazard>(out var hazard))
        {
            Debug.Log($"Player collided with hazard: {hazard.name}");
            //handle hazard collision cases
        }
    }

    public override void Move()
    {
        //camera and player body movement
        Vector2 dir = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(dir.x, 0, dir.y) * Speed * Time.deltaTime;

        Vector2 look = orientationAction.ReadValue<Vector2>() * lookSensitivity;
        yaw += look.x;
        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

    }

    public void OnCollisionExit(Collision collision)
    {
        HazardCollide(collision);        
        // throw new NotImplementedException();
        //check other cases of collision
        if (collision.gameObject.TryGetComponent<NPC>(out var npc))
        {
            Interact(collision.gameObject.GetComponent<Actor>());
        } 

    }

    public void UseItem(Item item)
    {
        item.Action();
    }

    //interact with another actor
    public void Interact(Actor actor)
    {
        //assume actor is an npc for now since it's really the only interactable thing
        actor.OutputDialogue();
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    public void UseCurrency(int amount)
    {
        Currency -= amount;
    }

    public void UseSkill(string skill)
    {
        if (skills.Contains(skill))
        {
            skills.Remove(skill);
            /*
                skill would be the one we want to implement
                not really sure how we would do this
                SkillSystem is not setup rn
             */
        }
    }
}
