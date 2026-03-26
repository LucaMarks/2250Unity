using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//manage NPC dialogue display and behaviour
//attatch to canvas UI in Unity
public class DialogueSystem : MonoBehaviour
{
    //references to UI
    public GameObject dialogueBox;//panel
    public TextMeshProUGUI npcNameText;//who
    public TextMeshProUGUI dialogueText;//what
    public TextMeshProUGUI[] optionTexts;//2/3 response buttons
    
    //dialogue states public so NPC can check if dialogue is already open
    public bool isDialogueActive = false;
    private int currentDialogueIndex = 0;
    private int selectedOptionIndex = 0;
    private NPC currentNPC;
    private List<DialogueLine> currentDialogueTree;//stores a conversation tree which will be hard coded for now

    void Start()
    {
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);//start and initialize
        }
    }
    
    //will be called when the player presses E on an NPC
    public void StartDialogue(NPC npc)
    {
        if (npc == null) return;
        currentNPC = npc;
        currentDialogueTree = npc.GetDialogueTree(); //get this NPC's dialogue lines
        currentDialogueIndex = 0; //start at the beginning
        selectedOptionIndex = 0; //highlight first option
        isDialogueActive = true;
        
        //show box on screen
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
        }
        
        //display initial dialogue
        DisplayCurrentLine();
    }

    void Update()
    {
        if (!isDialogueActive) return;
        
        //arrow Up move to previous option
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedOptionIndex--;
            if (selectedOptionIndex < 0)
            {
                selectedOptionIndex = 0;//don't go above first option
            }
            HighlightSelectedOption();//visually show which option is selected
        }

        //arrow Down move to next option
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            //figure out how far down we can go, how many options are there
            DialogueLine currentLine = currentDialogueTree[currentDialogueIndex];
            int maxOptions = currentLine.options.Count;

            selectedOptionIndex++;
            if (selectedOptionIndex >= maxOptions)
            {
                selectedOptionIndex = maxOptions - 1;//don't go past last option
            }
            HighlightSelectedOption();//show
        }

        //use E to select option
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            AdvanceDialogue(selectedOptionIndex);
        }
    }

    private void DisplayCurrentLine()
    {
        //check if we are at end
        if (currentDialogueIndex >= currentDialogueTree.Count)
        {
            EndDialogue();
            return;
        }
        
        DialogueLine currentLine = currentDialogueTree[currentDialogueIndex];

        //check if this line awards XP (positive path reward)
        if (currentLine.xpReward > 0 && currentNPC != null && !currentNPC.hasAwardedXP)
        {
            ProgressionSystem progression = FindFirstObjectByType<ProgressionSystem>();
            if (progression != null)
            {
                progression.AwardNpcInteractionXP();
                currentNPC.hasAwardedXP = true;//prevent farming XP by talking again
            }
        }

        //show name and dialogue text
        if (npcNameText != null) { npcNameText.text = currentLine.npcName; }
        if (dialogueText != null) { dialogueText.text = currentLine.dialogueText; }
        
        //player options
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (i < currentLine.options.Count)
            {
                optionTexts[i].text = currentLine.options[i].text;//show options
                optionTexts[i].gameObject.SetActive(true);
            }
            else { optionTexts[i].gameObject.SetActive(false); }
        }
        
        //reset selection to first option
        selectedOptionIndex = 0;
        HighlightSelectedOption();
    }
    private void HighlightSelectedOption()
    {
        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (i == selectedOptionIndex)
            {
                //make selected option yellow to show it's highlighted
                optionTexts[i].color = Color.yellow;
            }
            else
            {
                //otherwise it is white (normal/unselected)
                optionTexts[i].color = Color.white;
            }
        }
    }
    
    //advance dialogue based on player choice
    private void AdvanceDialogue(int optionChosen)
    {
        DialogueLine currentLine = currentDialogueTree[currentDialogueIndex];
        
        //get player option
        if (optionChosen >= currentLine.options.Count) { return; }//invalid
        DialogueOption selectedOption = currentLine.options[optionChosen];
        
        currentDialogueIndex = selectedOption.nextLineIndex;//move forward
        
        //if nextLineIndex is -1 the dialogue is over
        if (currentDialogueIndex == -1)
        {
            EndDialogue();
            return;
        }
        DisplayCurrentLine();//display the next line
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        selectedOptionIndex = 0;//reset option index

        //hide the dialogue box
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }

        currentNPC = null;
        currentDialogueTree = null;
    }
}

//simple structure to represent one dialogue line with NPC text and player options
public class DialogueLine
{
    public string npcName;//who
    public string dialogueText;//what they say
    public List<DialogueOption> options;//list of responses
    public int xpReward = 0;//if greater than 0, awards XP when this line is displayed

    public DialogueLine(string name, string text)
    {
        npcName = name;
        dialogueText = text;
        options = new List<DialogueOption>();
    }
}

//one player response option that leads to the next dialogue line
public class DialogueOption
{
    public string text;//what the player says
    public int nextLineIndex;//index of the next DialogueLine in the tree (-1 means end dialogue)

    public DialogueOption(string optionText, int nextIndex)
    {
        text = optionText;
        nextLineIndex = nextIndex;
    }
}