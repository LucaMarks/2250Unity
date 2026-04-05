using UnityEngine;

// Attach to the stone pillar GameObjects in each chamber
// Make sure the GameObject is tagged "Pillar" in the Unity Inspector
// Activates when the light beam hits it
// Tracks how many pillars are active using static counters
public class Pillar : MonoBehaviour
{
    public bool isActivated = false;
    public Material activatedMaterial; // drag a glowing/lit material in the inspector

    private Renderer pillarRenderer;

    // static counters shared across all Pillar instances in the scene
    public static int activatedCount = 0;
    public static int totalPillars = 0;

    void Start()
    {
        pillarRenderer = GetComponent<Renderer>();
        totalPillars++;
    }

    void OnDestroy()
    {
        // clean up count when scene unloads
        totalPillars--;
        if (isActivated) activatedCount--;
    }

    // called by LightBeam when the beam hits this pillar
    public void ActivatePillar()
    {
        if (isActivated) return; // already activated, ignore

        isActivated = true;
        activatedCount++;

        Debug.Log($"Pillar activated! {activatedCount}/{totalPillars} pillars lit.");

        // swap to the activated material to show it's been lit
        if (activatedMaterial != null && pillarRenderer != null)
        {
            pillarRenderer.material = activatedMaterial;
        }

        // check if every pillar in this scene is now active
        if (activatedCount >= totalPillars)
        {
            OnAllPillarsActivated();
        }
    }

    private void OnAllPillarsActivated()
    {
        Debug.Log("All pillars activated! Chamber unsealed.");
        // TODO: hook this up to open a door or trigger a cave-in event
        // will be implemented when level layout is built
    }

    // call this when loading a new scene to reset the static counters
    public static void ResetCounters()
    {
        activatedCount = 0;
        totalPillars = 0;
    }
}
