using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Inventory
{
    private List<Item> items = new List<Item>();
    private int maxSlots;
    private int itemsIndex = 0;

    private ItemFrequency[] itemFrequencies = new ItemFrequency[20];
    private int itemFreqSize = 0;

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
    }
    public Inventory(){}

    public bool hasItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
            {
                return true;
            }
        }

        return false;
    }

    public void removeItem(Item item)
    {
        if (hasItem(item))
        {
            items.Remove(item);
            itemsIndex--;
        }
    }

    public bool addItem(Item item)
    {
        //two cases
        // -> Item already exists in players inventory
        // -> item is not in player's inventory

        if (!hasItem(item))
        {
            // Debug.Log("First Item added");
            items.Add(item);
            // frequen
            int freqIndex = hasFrequency(item);
            if (freqIndex != -1)
            {
                itemFrequencies[freqIndex].currStorage++;
            }
            itemsIndex++;
            return true;
        }

        for (int i = 0; i < itemFreqSize; i++)
        {
            if (itemFrequencies[i].item.ID == item.ID)
            {
                if (itemFrequencies[i].currStorage < itemFrequencies[i].maxStorage)
                {
                    items.Add(item);
                    return true;
                }else{return false;}
            }

            // if (i + 1 == itemFrequencies.Count)
            // {
            //     return false;
            // }
        }
        // items.Add(item);
        itemsIndex++;
        return true;
    }

    public int getLen()
    {
        return items.Count;
    }

    public Item getItem(int slot)
    {
        return items[slot];
    }

    public void addFrequency(ItemFrequency item)
    {
        // if (item != null){Debug.Log("Not null here!");}
        itemFrequencies[itemFreqSize] = item;
        itemFreqSize++;
        // Debug.Log(itemFrequencies[itemFreqSize-1].item.ID + " at index " + (itemFreqSize-1));
        Debug.Log(itemFrequencies[itemFreqSize-1].item.ID + " at index " + (itemFreqSize-1) + " of size " + itemFrequencies[itemFreqSize-1].maxStorage);
        // Debug.Log(itemFrequencies[itemFreqSize-1].item);
    }

    public void removeFrequency(ItemFrequency item)
    {
        int index = hasFrequency(item.item);
        if (index != -1)
        {
            itemFrequencies[index] = null;
            // itemFrequencies.Remove(item);
            itemFreqSize--;
        }
    }

    public ItemFrequency getFrequency(int slot)
    {
        return itemFrequencies[slot];
    }

    private int hasFrequency(Item item)
    {
        // Debug.Log(itemFreqSize);
        for (int i = 0; i < itemFreqSize; i++)
        {
            Debug.Log(i);
            if (itemFrequencies[i].item.ID == item.ID)
            {
                return i;
            }
        }

        return -1;
    }
}