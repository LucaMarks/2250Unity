using UnityEngine;

public class PlayerSkygliderState : MonoBehaviour
{
    [Header("Skyglider State")]
    public bool hasSkyglider;
    public Skyglider equippedSkyglider;
    public bool isMounted;
    public Skyglider mountedSkyglider;

    public void GrantSkyglider(Skyglider skyglider)
    {
        hasSkyglider = true;
        equippedSkyglider = skyglider;
        Debug.Log("PlayerSkygliderState -> GrantSkyglider called. hasSkyglider=true");
    }

    public void RemoveSkyglider()
    {
        hasSkyglider = false;
        equippedSkyglider = null;
        isMounted = false;
        mountedSkyglider = null;
        Debug.Log("PlayerSkygliderState -> RemoveSkyglider called.");
    }

    public bool HasSkyglider()
    {
        return hasSkyglider;
    }

    public void MountSkyglider(Skyglider skyglider)
    {
        isMounted = true;
        mountedSkyglider = skyglider;
        hasSkyglider = true;
        equippedSkyglider = skyglider;
        Debug.Log("PlayerSkygliderState -> MountSkyglider called. isMounted=true");
    }

    public void DismountSkyglider()
    {
        isMounted = false;
        mountedSkyglider = null;
        Debug.Log("PlayerSkygliderState -> DismountSkyglider called. isMounted=false");
    }
}
