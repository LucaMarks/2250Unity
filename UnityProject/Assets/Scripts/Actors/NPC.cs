using System.Collections.Generic;
using UnityEngine;

public class NPC : StaticObject
{
    private string Name;

    private string Dialogue;

    public List<object> Quests; // quest class not yet created
    private ActorState currentState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //quest logic will go here
    }

    void Drop()
    {
        //after interaction (if defined) will give an item (currency for example)
    }
}
