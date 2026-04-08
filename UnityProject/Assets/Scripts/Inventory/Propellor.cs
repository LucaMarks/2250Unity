using UnityEngine;

public class Propellor : Item
{
    public static bool WasCollected;

    private void Awake()
    {
        WasCollected = false;
        ID = "Propellor";
        Name = "Propellor";

        if (itemContainer == null)
        {
            itemContainer = gameObject;
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            WasCollected = true;
        }
    }
}
