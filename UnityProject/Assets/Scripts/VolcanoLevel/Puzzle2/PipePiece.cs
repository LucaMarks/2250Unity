using UnityEngine;

public class PipePiece : MonoBehaviour
{
    public int currentRotation = 0; // positions 1, 2, 3, or 4
    public int correctRotation = 0; // set in Inspector

    public float rotationSpeed = 200f;
    private bool isRotating = false;

    private Vector3 initialRotation;

    public void Rotate()
    {
        if (!isRotating)
        {

            StartCoroutine(RotateSmooth());
        }
    }

    System.Collections.IEnumerator RotateSmooth()
    {
        isRotating = true;
        float startY = transform.eulerAngles.y;
        float targetY = startY + 90f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / 90f);
            float y = Mathf.Lerp(startY, targetY, t);
            transform.eulerAngles = new Vector3(initialRotation.x, y, initialRotation.z);
            yield return null;
        }

        currentRotation = (currentRotation + 1) % 4;

        isRotating = false;
    }

    public bool IsCorrect()
    {
        
        return currentRotation == correctRotation;
    }
    
    void Start()
    {
        initialRotation = transform.eulerAngles;
    }
}