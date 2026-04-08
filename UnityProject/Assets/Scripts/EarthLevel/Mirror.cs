using UnityEngine;
using UnityEngine.InputSystem;


// Player presses E when nearby to rotate the mirror 45 degrees (changable in unity)
public class Mirror : MonoBehaviour
{
    public float interactRange = 3f;  // how close player needs to be to rotate
    public float rotationStep = 45f;  // how many degrees it rotates per E press

    private Player player;

    void Start()
    {
        // cache the player reference once at start
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // only respond to E key if player is close enough
        if (distance <= interactRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            RotateMirror();
        }
    }

    private void RotateMirror()
    {
        // rotate around the Y axis by the rotation step
        transform.Rotate(0, rotationStep, 0);
        Debug.Log(gameObject.name + " rotated to " + transform.eulerAngles.y + " degrees");
    }
}
