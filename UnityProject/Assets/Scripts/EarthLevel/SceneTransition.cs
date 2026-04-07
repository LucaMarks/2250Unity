using UnityEngine;
using UnityEngine.SceneManagement;

// Attach to an invisible trigger zone at the end of each chamber
// Set the Collider to "Is Trigger" in the Inspector
// Set the nextSceneName in the Inspector to the next scene (e.g. "EarthLevel2")
// Optionally require all pillars to be activated before allowing transition
public class SceneTransition : MonoBehaviour
{
    public string nextSceneName; // name of the next scene to load
    public bool requireAllPillarsActivated = true; // must solve puzzle to proceed

    private void OnTriggerEnter(Collider other)
    {
        // only respond to the player entering the trigger zone
        if (!other.CompareTag("Player")) return;

        // check if puzzle is solved before allowing progression
        if (requireAllPillarsActivated && Pillar.activatedCount < Pillar.totalPillars)
        {
            Debug.Log("All pillars must be activated before proceeding.");
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // reset pillar counters before loading new scene
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
