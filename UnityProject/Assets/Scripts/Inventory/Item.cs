using UnityEngine;

public class Item : StaticObject
{
    public string ID;
    public string Name;
    private string Description;
    public GameObject itemContainer;

    private int Value;

    public Item()
    {
        
    }

    public Item(string ID, string Name, string Description, int Value)
    {
        this.ID = ID;
        this.Name = Name;
        this.Description = Description;
        this.Value = Value;
    }
    public Item(string ID, string Name, int Value)
    {
        this.ID = ID;
        this.Name = Name;
        this.Value = Value;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        
    }

    public void Action()
    {
        //some effect
    }
}
