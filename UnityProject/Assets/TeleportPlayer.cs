using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    // target teleport position
    public Vector3 targetPosition = new Vector3(184.5f, 0f, -96.8f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If player has Rigidbody (recommended)
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.position = targetPosition;
                rb.linearVelocity = Vector3.zero; // stops weird movement after teleport
            }
            else
            {
                other.transform.position = targetPosition;
            }
        }
    }
}