using UnityEngine;

public class LavaRise : MonoBehaviour
{
    public float riseSpeed = 2f;
    public float maxHeight = 10f;

    private bool isRising = false;

    public void StartRising()
    {
        isRising = true;
    }

    void Update()
    {
        if (isRising)
        {
            if (transform.position.y < maxHeight)
            {
                transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            }
            else
            {
                isRising = false;
                Debug.Log("Lava reached max height!");
            }
        }
    }
}