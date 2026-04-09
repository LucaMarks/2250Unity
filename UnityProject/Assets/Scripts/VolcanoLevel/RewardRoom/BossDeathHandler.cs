using UnityEngine;

public class BossDeathHandler : MonoBehaviour
{
    public BossDoor door;
    public KillRedDragon quest;

    private EnemyAI enemy;

    void Start()
    {
        enemy = GetComponent<EnemyAI>();
    }

    public void OnBossDeath()
    {
        if (door != null)
        {
            door.OpenDoor();
        }

        if (quest != null)
        {
            quest.OnDragonKilled();
        }
    }
}
