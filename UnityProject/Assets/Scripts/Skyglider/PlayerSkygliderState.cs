using UnityEngine;

public class PlayerSkygliderState : MonoBehaviour
{
    [Header("Skyglider State")]
    public bool hasSkyglider;
    public Skyglider equippedSkyglider;
    public bool isMounted;
    public Skyglider mountedSkyglider;
    private Player cachedPlayer;

    private void Awake()
    {
        cachedPlayer = GetComponent<Player>();
    }

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
        SetWeaponVisible(false);
        Debug.Log("PlayerSkygliderState -> MountSkyglider called. isMounted=true");
    }

    public void DismountSkyglider()
    {
        isMounted = false;
        mountedSkyglider = null;
        SetWeaponVisible(true);
        Debug.Log("PlayerSkygliderState -> DismountSkyglider called. isMounted=false");
    }

    private void SetWeaponVisible(bool isVisible)
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = GetComponent<Player>();
        }

        if (cachedPlayer == null || cachedPlayer.sword == null)
            return;

        cachedPlayer.sword.SetActive(isVisible);
    }
}
