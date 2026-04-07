using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PressurePlate[] plates;
    public bool isSolved = false;

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
        }
    }
}