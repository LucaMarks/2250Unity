using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public double QuestID;

    public List<Item> Rewards;

    public bool Completion = false;
    
    public bool flag = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetCompleted(flag);
    }

    public bool IsCompleted()
    {
        return Completion;
    }

    public void SetCompleted(bool flg)
    {
        if (flg)
        {
            Completion = true;
        }
    }
}
