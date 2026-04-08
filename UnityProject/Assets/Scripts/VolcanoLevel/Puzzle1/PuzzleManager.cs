using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PressurePlate[] plates;
    public bool isSolved = false;

    public Material green;
    public Renderer button;
    
    void Update()
    {
        CheckPuzzle();
    }

    void CheckPuzzle()
    {
        foreach (var plate in plates)
        {
            if (!plate.isActivated)
            {
                isSolved = false;
                return;
            }
        }

        if (!isSolved)
        {
            isSolved = true;
            Debug.Log("Box Puzzle Solved!");
            ActivateButton();
        }
    }
    
    void ActivateButton()
    {
        if(button != null && green != null)
        {
            button.material = green;
        }
    }
}