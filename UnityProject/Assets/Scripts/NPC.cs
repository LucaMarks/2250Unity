using System.Collections.Generic;
using UnityEngine;

public class NPC : StaticObject
{
    private string Name;

    private string Dialogue;
    //DialogueSystem functionality
    private List<DialogueLine> dialogueTree;
    private DialogueSystem dialogueSystem;

    public List<object> Quests; // quest class not yet created
    private ActorState currentState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //implement dialogue for NPC
        dialogueSystem = FindObjectOfType<DialogueSystem>();
        InitializeDialogueTree();
    }

    // Update is called once per frame
    void Update()
    {
        //quest logic will go here
        
        //interaction logic
        if (Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            CheckIfPlayerLookingAtMe();
        }
    }
    
    private void InitializeDialogueTree()
    {
        dialogueTree = new List<DialogueLine>();
        //TODO: add dialogue lines here later
    }

    private void CheckIfPlayerLookingAtMe()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null || dialogueSystem == null) return;

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                dialogueSystem.StartDialogue(this);
            }
        }
    }

    public List<DialogueLine> GetDialogueTree()
    {
        return dialogueTree;
    }
    
    public void StartDialogue()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(this);
        }
    }

    void Drop()
    {
        //after interaction (if defined) will give an item (currency for example)
    }
}
