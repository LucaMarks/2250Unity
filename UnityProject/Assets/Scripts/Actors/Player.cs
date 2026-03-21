
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor //this also gives us access to MonoBehavoiour
{
    public int Currency = 100;//default currency
    private List<string> skills;
    public Inventory inventory = new Inventory();
    private int SkillPoints;

    public Player(int speed, int health, int damage, List<List<int>> position) : base(health, damage, position)
    {
        //
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