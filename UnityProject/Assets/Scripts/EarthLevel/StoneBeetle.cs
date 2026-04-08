using UnityEngine;

//earth level unique enemy - the Stone Beetle
//when killed, splits into 2 smaller Stone Beetles
//small beetles do not split again when killed
//extends EnemyAI to reuse all patrol, chase, and attack logic
public class StoneBeetle : EnemyAI
{
    public GameObject smallBeetlePrefab;
    public bool isSmallBeetle = false;   // if true, this beetle won't split on death
    public int splitCount = 2;           // how many small beetles spawn on death

    protected override void DestroyEnemy()
    {
        // only split if this is a full-size beetle and a prefab is assigned
        if (!isSmallBeetle && smallBeetlePrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                // spawn each small beetle at a slightly random offset so they don't stack
                Vector3 spawnOffset = new Vector3(
                    Random.Range(-1f, 1f),
                    0,
                    Random.Range(-1f, 1f)
                );

                GameObject smallBeetle = Instantiate(
                    smallBeetlePrefab,
                    transform.position + spawnOffset,
                    Quaternion.identity
                );

                // mark the spawned beetle as small so it won't split again
                StoneBeetle smallBeetleScript = smallBeetle.GetComponent<StoneBeetle>();
                if (smallBeetleScript != null)
                {
                    smallBeetleScript.isSmallBeetle = true;
                }
            }

            Debug.Log(gameObject.name + " split into " + splitCount + " smaller beetles!");
        }

        // call the base EnemyAI destroy to handle normal cleanup
        base.DestroyEnemy();
    }
}
