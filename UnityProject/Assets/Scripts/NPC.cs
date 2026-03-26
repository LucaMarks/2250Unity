using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        InitializeDialogueTree();
    }

    // Update is called once per frame
    void Update()
    {
        //quest logic will go here
        
        //interaction logic
        if (Keyboard.current.eKey.wasPressedThisFrame && (dialogueSystem == null || !dialogueSystem.isDialogueActive))
        {
            CheckIfPlayerLookingAtMe();
        }
    }
    
    private void InitializeDialogueTree()
    {
        dialogueTree = new List<DialogueLine>();

        //line 0 opening line
        //options: ask what's for sale (line 1) or leave (end)
        DialogueLine line0 = new DialogueLine("Merchant", "Welcome traveler, what brings you to my shop?");
        line0.options.Add(new DialogueOption("What do you have for sale?", 1));
        line0.options.Add(new DialogueOption("I don't want anything, thanks.", -1));
        dialogueTree.Add(line0);

        //line 1 merchant describes wares
        //options: buy a potion (line 2) or leave (end)
        DialogueLine line1 = new DialogueLine("Merchant", "I have health potions and a fine sword. Either could save your life out there.");
        line1.options.Add(new DialogueOption("I'll take a health potion.", 2));
        line1.options.Add(new DialogueOption("Too expensive for me.", -1));
        dialogueTree.Add(line1);

        //line 2 purchase confirmed, end of conversation
        //only one option to wrap up (end)
        DialogueLine line2 = new DialogueLine("Merchant", "Good choice! That'll restore your health. Safe travels, adventurer.");
        line2.options.Add(new DialogueOption("Thank you!", -1));
        dialogueTree.Add(line2);
    }

    private void CheckIfPlayerLookingAtMe()
    {
        Player player = FindFirstObjectByType<Player>();
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
