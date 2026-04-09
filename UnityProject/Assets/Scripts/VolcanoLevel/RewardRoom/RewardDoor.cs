using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public GameObject door; // the cube to delete

    public void OpenDoor()
    {
        Debug.Log("Door opened!");
        Destroy(door);
    }
}