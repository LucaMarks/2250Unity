using System.Collections.Generic;
using Unity.VisualScripting;


public class Inventory
{
    List<Item> items = new List<Item>();
    private int maxSlots;

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
    }
    public Inventory(){}

    public bool hasItem(Item item)
    {
        if (items.Contains(item))
        {
            return true;
        }

        return false;
    }

    public void removeItem(Item item)
    {
        if (hasItem(item))
        {
            items.Remove(item);
        }
    }

    public void addItem(Item item)
    {
        items.Add(item);
    }
}