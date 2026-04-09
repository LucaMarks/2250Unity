using UnityEngine;
using UnityEngine.SceneManagement;

//Attach to an invisible trigger zone at the end of each chamber
public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;
    public bool requireAllPillarsActivated = true;

    //tick this on the EarthLevel3 exit trigger only
    public bool requireGolemDefeated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        //check puzzle
        if (requireAllPillarsActivated && Pillar.activatedCount < Pillar.totalPillars)
        {
            Debug.Log("All pillars must be activated before proceeding.");
            return;
        }

        //check golem (level will be ticked 3 only)
        if (requireGolemDefeated && !EarthGolem.isDefeated)
        {
            Debug.Log("The Earth Golem must be defeated before you can leave.");
            return;
        }

        //all conditions met, give earth gemstone if this is the final level exit
        if (requireGolemDefeated)
        {
            Player player = other.GetComponent<Player>();
            if (player == null) player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                Item earthGem = new Item("EarthGemstone", "Earth Gemstone", 100);
                player.inventory.addItem(earthGem);
                Debug.Log("Earth Gemstone added to inventory!");
            }
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Pillar.ResetCounters();
            Debug.Log("Transitioning to " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransition: no scene name set in the Inspector!");
        }
    }
}
