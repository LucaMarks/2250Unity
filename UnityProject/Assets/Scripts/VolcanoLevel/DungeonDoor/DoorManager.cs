using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public PuzzleManager boxPuzzle;
    public PipePuzzleManager pipePuzzle;

    private bool isOpen = false;

    void Update()
    {
        if (!isOpen && boxPuzzle.isSolved && pipePuzzle.isSolved)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Door Opened!");

        // Option 1: Disable door (simple)
        gameObject.SetActive(false);
    }
}