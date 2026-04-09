using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    public Transform leverFlip;
    public Renderer targetCube;
    public Material green;

    public bool isActivated = false;

    public void Interact()
    {
        if (isActivated) return;

        isActivated = true;

        // Rotate lever 180 degrees on Y
       // leverFlip.localRotation = Quaternion.Euler(0f, 180f, 0f);
        leverFlip.Rotate(0f, 180f, 0f);
        ActivateButton();
    }

    void ActivateButton()
    {
        if (targetCube != null && green != null)
        {
            targetCube.material = green;
        }
    }
}