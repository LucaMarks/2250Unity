using UnityEngine;

public class LavaTrigger : MonoBehaviour
{
    public LavaRise lava;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Lava Rising Triggered!");

            lava.StartRising();
        }
    }
}
