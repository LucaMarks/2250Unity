using System;
using UnityEngine;

public abstract class Quest : MonoBehaviour
{


    [Header("Quest Info")]
    public string questName;

    [TextArea(2, 5)]
    public string description;

    public UpdatedNPC associatedNPC;
    public DialogueStage dialogueToUnlock;

    [Header("Quest State")]
    public bool hasStarted;
    public bool isCompleted;

    public Item[] gemstoneRewards = new Item[4];
    public Item waterGemstone= new Item("WaterGemstone", "Water Gemstone", 100);
    public Item airGemstone= new Item("AirGemstone", "Air Gemstone", 100);
    public Item fireGemstone= new Item("FireGemstone", "Fire Gemstone", 100);
    public Item iceGemstone= new Item("IceGemstone", "Ice Gemstone", 100);
    public Item earthGemstone = new Item("Earth Gemstone", "Earth Gemstone", 100);


    //hardcoded gemstone rewards
    public void Start()
    {
        // setRewards();
        // throw new NotImplementedException();
    }

    private void setRewards()
    {
        gemstoneRewards[0] = new Item("WaterGemstone", "Water Gemstone", 100);
        gemstoneRewards[1] = new Item("AirGemstone", "Air Gemstone", 100);
        gemstoneRewards[2] = new Item("FireGemstone", "Fire Gemstone", 100);
        gemstoneRewards[3] = new Item("IceGemstone", "Ice Gemstone", 100);
        gemstoneRewards[4] = new Item("Earth Gemstone", "Earth Gemstone", 100);
    }
    public virtual void StartQuest()
    {
        // setRewards();

        if (hasStarted || isCompleted)
            return;

        hasStarted = true;
        Debug.Log("Started quest: " + questName);
    }

    public virtual void UpdateQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        if (CheckIfComplete())
        {
            CompleteQuest();
        }
    }

    public abstract string GetObjectiveText();

    public abstract bool CheckIfComplete();

    public virtual void CompleteQuest()
    {
        if (!hasStarted || isCompleted)
            return;

        if (!CheckIfComplete())
            return;

        QuestManager.Instance.ClearCurrentQuest();
        isCompleted = true;
        dialogueToUnlock.isLocked = false;
        GiveReward();
        // associatedNPC.

        Debug.Log("Completed quest: " + questName);
    }

    protected virtual void GiveReward()
    {
        Debug.Log("Reward given for quest: " + questName);
    }
}