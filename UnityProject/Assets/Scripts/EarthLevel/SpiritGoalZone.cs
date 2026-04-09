using UnityEngine;


//when the Earth Spirit enters this zone, the side quest is marked complete.
public class SpiritGoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //check if the Earth Spirit walked into this zone
        EarthSpiritFollow spirit = other.GetComponent<EarthSpiritFollow>();
        if (spirit == null)
            spirit = other.GetComponentInParent<EarthSpiritFollow>();

        if (spirit != null && !EarthSideQuest.spiritReachedGoal)
        {
            EarthSideQuest.spiritReachedGoal = true;
            Debug.Log("Earth Spirit reached the goal zone! Side quest objective complete.");

            //QuestManager's Update loop calls currentQuest.UpdateQuest() every frame,
            //so it will pick up spiritReachedGoal = true automatically and complete the quest.
        }
    }
}
