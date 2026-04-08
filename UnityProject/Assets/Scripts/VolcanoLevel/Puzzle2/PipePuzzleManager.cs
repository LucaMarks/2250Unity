using UnityEngine;

public class PipePuzzleManager : MonoBehaviour
{
    public PipePiece[] pipes;
    public bool isSolved = false;

    public Material lava;
    public Renderer lavaRenderer;
    
    void Update()
    {
        CheckPuzzle();
    }

    void CheckPuzzle()
    {
        foreach (var pipe in pipes)
        {
            if (!pipe.IsCorrect())
            {
                isSolved = false;
                return;
                
            }
        }

        if (!isSolved)
        {
            isSolved = true;
            Debug.Log("Pipe Puzzle Solved!");

            ActivateLava();
        }
    }

    void ActivateLava()
    {
        if(lavaRenderer != null && lava != null)
        {
            lavaRenderer.material = lava;
        }
    }
}