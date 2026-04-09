using UnityEngine;

public class TeleportToPosition : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Destination")]
    public Vector3 targetPosition = new Vector3(184.5f, 0f, -96.8f);

    public void Teleport()
    {
        if (target == null)
        {
            Debug.LogWarning("TeleportToPosition -> No target assigned.");
            return;
        }

        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody == null)
        {
            targetRigidbody = target.GetComponentInParent<Rigidbody>();
        }

        if (targetRigidbody != null)
        {
            targetRigidbody.linearVelocity = Vector3.zero;
            targetRigidbody.angularVelocity = Vector3.zero;
        }

        target.position = targetPosition;

        Debug.Log("TeleportToPosition -> Teleported " + target.name + " to " + targetPosition);
    }
}
