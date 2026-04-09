using UnityEngine;

public class WizardBoots : Item
{
    private void Awake()
    {
        ID = "WizardBoots";
        Name = "Wizard Boots";

        if (itemContainer == null)
        {
            itemContainer = gameObject;
        }
    }
}
