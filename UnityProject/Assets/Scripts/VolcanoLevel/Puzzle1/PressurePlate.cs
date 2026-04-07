using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public string requiredColour; // "Any required colour"
    public bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Box>(out var box))
        {
            if (box.boxColour == requiredColour)
            {
                isActivated = true;
                Debug.Log(requiredColour + " plate activated!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Box>(out var box))
        {
            if (box.boxColour == requiredColour)
            {
                isActivated = false;
                Debug.Log(requiredColour + " plate deactivated!");
            }
        }
    }
}