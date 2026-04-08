using UnityEngine;

public class SkyRudderItem : Item
{
    public static bool WasCollected;

    private void Awake()
    {
        WasCollected = false;
        ID = "Rudder";
        Name = "Rudder";

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
