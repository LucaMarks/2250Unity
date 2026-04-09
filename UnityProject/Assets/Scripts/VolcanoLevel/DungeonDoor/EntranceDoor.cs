using UnityEngine;

public class EntranceDoor : MonoBehaviour
{
    public LeverSwitch[] switches;
    public GameObject door;

    private bool doorOpened = false;

    void Update()
    {
        if (doorOpened) return;

        if (AllSwitchesActivated())
        {
            OpenDoor();
        }
    }

    bool AllSwitchesActivated()
    {
        foreach (LeverSwitch s in switches)
        {
            if (!s.isActivated)
                return false;
        }
        return true;
    }

    void OpenDoor()
    {
        doorOpened = true;
        Destroy(door);
    }
}