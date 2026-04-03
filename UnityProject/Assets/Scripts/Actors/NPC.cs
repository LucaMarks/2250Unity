#if false
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
    public bool hasAwardedXP = false;//tracks if this NPC already gave the player XP (prevents farming)
    private Player player;//cached player reference so we don't search the scene every frame

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //implement dialogue for NPC
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        player = FindFirstObjectByType<Player>();//cache once at start instead of every frame
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

        // line 0 - opening greeting (depth 1)
        DialogueLine line0 = new DialogueLine("Elder Sage", "Ah, a knight bearing the king's crest... You seek the elemental gemstones, do you not?");
        line0.options.Add(new DialogueOption("Yes, the king sent me to collect them.", 1));
        line0.options.Add(new DialogueOption("What do you know about the gemstones?", 2));
        line0.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line0);

        // line 1 - about the king's orders (depth 2)
        DialogueLine line1 = new DialogueLine("Elder Sage", "The king sends many knights on this errand, yet few return. Have you wondered why he wants these stones so badly?");
        line1.options.Add(new DialogueOption("What happened to the other knights?", 3));
        line1.options.Add(new DialogueOption("Why would the king lie to me?", 4));
        line1.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line1);

        // line 2 - about the gemstones (depth 2)
        DialogueLine line2 = new DialogueLine("Elder Sage", "Five elemental gemstones... Ice, Fire, Earth, Water, and Air. They were forged long ago by an ancient civilization to seal away a great darkness. Each is guarded by a powerful elemental being.");
        line2.options.Add(new DialogueOption("An ancient civilization? Tell me more.", 3));
        line2.options.Add(new DialogueOption("Why are the guardians protecting them?", 4));
        line2.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line2);

        // line 3 - about the ancient civilization and ruins (depth 3)
        DialogueLine line3 = new DialogueLine("Elder Sage", "Deep beneath the earth lie the ruins of the ones who forged the gemstones. Their cities crumbled, but their guardians remain, bound by duty to protect the stones at any cost. Some say their spirits still wander those underground halls.");
        line3.options.Add(new DialogueOption("What does the king want with that power?", 5));
        line3.options.Add(new DialogueOption("What happens if all five are gathered?", 6));
        line3.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line3);

        // line 4 - about the guardians (depth 3)
        DialogueLine line4 = new DialogueLine("Elder Sage", "Each guardian is the last protector of its realm. Ice, Fire, Earth, Water, Air... they were chosen to keep the gemstones safe from those who would misuse their power. They will not surrender them willingly.");
        line4.options.Add(new DialogueOption("Who would misuse them?", 5));
        line4.options.Add(new DialogueOption("What happens if all five are united?", 6));
        line4.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line4);

        // line 5 - the king's dark truth (depth 4)
        DialogueLine line5 = new DialogueLine("Elder Sage", "The king does not seek mere treasure, knight. He seeks dominion. With all five gemstones, one could command the very elements and bend every realm to their will. The people he rules over already suffer under his greed.");
        line5.options.Add(new DialogueOption("Is there another choice for me?", 7));
        line5.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line5);

        // line 6 - what happens when united (depth 4)
        DialogueLine line6 = new DialogueLine("Elder Sage", "The ancient ones tried to unite the five gemstones once before. The power was too great and it tore their civilization apart, burying it beneath the earth. If the king gathers all five... history may repeat itself, but this time the whole world pays the price.");
        line6.options.Add(new DialogueOption("Can anything be done to stop this?", 7));
        line6.options.Add(new DialogueOption("I've heard enough, see you later.", -1));
        dialogueTree.Add(line6);

        // line 7 - final warning (depth 5)
        // player can leave normally OR show wisdom to earn XP
        DialogueLine line7 = new DialogueLine("Elder Sage", "When the time comes and you hold all five gemstones, you will face a choice. Return them to the king, or share the riches with the people who helped you along the way. Not all who wear crowns deserve loyalty. Remember my words, knight.");
        line7.options.Add(new DialogueOption("Your wisdom has opened my eyes. I swear I will protect the realms, not serve the king's greed.", 8));
        line7.options.Add(new DialogueOption("Thank you for the warning, elder.", -1));
        dialogueTree.Add(line7);

        // line 8 - positive path reward, elder is impressed (depth 5)
        DialogueLine line8 = new DialogueLine("Elder Sage", "Few knights have the courage to question their king, and fewer still have the heart to stand for the people. You remind me of the ancient guardians... Take this knowledge with you. It will strengthen your resolve for the trials ahead.");
        line8.options.Add(new DialogueOption("I won't forget your teachings, elder.", 9));
        dialogueTree.Add(line8);

        // line 9 - narrative XP reward message, no speaker name (forced end)
        // blank name so the NPC name field shows nothing, feels like inner narration
        DialogueLine line9 = new DialogueLine("", "You feel your heart shift... something has changed within you. *Gained 25 XP*");
        line9.xpReward = 25;
        line9.options.Add(new DialogueOption("...", -1));
        dialogueTree.Add(line9);
    }

    private void CheckIfPlayerLookingAtMe()
    {
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
#endif  