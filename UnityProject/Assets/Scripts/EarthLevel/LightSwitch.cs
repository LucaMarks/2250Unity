using UnityEngine;
using TMPro;

//Attach to the lever/switch object the player interacts with to fire the beam
public class LightSwitch : MonoBehaviour
{
    [Header("References")]
    public LightBeam lightBeam;

    [Header("Settings")]
    public float interactRange = 3f;
    public float cooldown = 7f;//3s longer then beam time to prevent spam

    private Player player;
    private float lastActivatedTime = -999f;//placeholder

    void Start()
    {
        player = FindFirstObjectByType<Player>();//cache player
    }

    void Update()
    {
        if (player == null || lightBeam == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);//check if player in interact range
        bool onCooldown = Time.time - lastActivatedTime < cooldown;//check if we are on a cooldown

        if (distance <= interactRange && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (onCooldown)
            {
                Debug.Log("Light switch is recharging...");
                return;
            }//no light

            lightBeam.ActivateBeam();//otherwise turn on
            lastActivatedTime = Time.time;//update the -999 var to this time
            Debug.Log("Beam activated! " + lightBeam.beamDuration + " seconds remaining.");
        }
    }
}
