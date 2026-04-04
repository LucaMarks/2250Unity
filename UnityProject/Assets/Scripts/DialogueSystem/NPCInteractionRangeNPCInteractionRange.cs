using UnityEngine;

public class NPCInteractionRange : MonoBehaviour
{
    public UpdatedNPC npc;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.SetCurrentNPC(npc);
            Debug.Log("Player entered range of: " + npc.npcName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.ClearCurrentNPC(npc);
            Debug.Log("Player left range of: " + npc.npcName);
        }
    }
}