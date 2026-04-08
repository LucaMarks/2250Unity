using UnityEngine;

public class EnergyCrystal : Item
{
    public static bool WasCollected;

    private void Awake()
    {
        WasCollected = false;
        ID = "EnergyCrystal";
        Name = "Energy Crystal";

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
