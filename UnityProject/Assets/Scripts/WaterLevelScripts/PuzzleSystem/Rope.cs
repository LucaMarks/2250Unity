using UnityEngine;

public class Rope :  MonoBehaviour
{
    public int dir;
    public StonePlatform stonePlatform;
    public StonePlatform[] platforms;

    public void Interact()
    {
        stonePlatform.movePlatform(dir);
        animateDown();
        if (dir == -1)
        {
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].Reset();
            }
        }
    }

    private void animateDown()
    {
        
    }


}