using UnityEngine;

public class Gemstone : Item
{
    public static bool WasCollected;

    private void Awake()
    {
        WasCollected = false;
        ID = "Gemstone";
        Name = "Gemstone Ruby";

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
