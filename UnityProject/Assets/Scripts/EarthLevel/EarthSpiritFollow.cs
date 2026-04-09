using UnityEngine;
using UnityEngine.AI;

//follow using navmesh
[RequireComponent(typeof(NavMeshAgent))]
public class EarthSpiritFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    //how close the spirit tries to stay to the player
    public float followDistance = 3f;
    //how far the player has to get before the spirit starts moving
    public float startMovingDistance = 4f;

    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;//stays still until quest starts

        //find the player automatically
        Player playerObj = FindFirstObjectByType<Player>();
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("EarthSpiritFollow: no Player found in scene.");
    }

    private void Update()
    {
        //only follow once the side quest has started and the goal hasn't been reached yet
        bool questActive = QuestManager.Instance != null
                        && QuestManager.Instance.currentQuest != null
                        && QuestManager.Instance.currentQuest.hasStarted
                        && !QuestManager.Instance.currentQuest.isCompleted;

        if (!questActive || EarthSideQuest.spiritReachedGoal)
        {
            agent.enabled = false;
            return;
        }

        if (player == null) return;

        //turn on NavMeshAgent once following begins
        if (!agent.enabled)
            agent.enabled = true;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //only move if player is far enough away to avoid jittering
        if (distanceToPlayer > startMovingDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            //close enough, dont move
            agent.ResetPath();
        }
    }
}
