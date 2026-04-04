using UnityEngine;
using TMPro;

public class UpdatedDialogueSystem : MonoBehaviour
{
    public static UpdatedDialogueSystem Instance;
    
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTalking;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        isTalking = false;
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        nameText.text = speakerName;
        currentLines = lines;
        currentLineIndex = 0;
        isTalking = true;

        dialoguePanel.SetActive(true);
        dialogueText.text = currentLines[currentLineIndex];
    }

    public void NextLine()
    {
        if (!isTalking)
            return;

        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        isTalking = false;
        dialoguePanel.SetActive(false);
    }

    public bool IsTalking()
    {
        return isTalking;
    }
}