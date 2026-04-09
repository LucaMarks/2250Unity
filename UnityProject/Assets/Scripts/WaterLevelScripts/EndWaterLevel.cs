using System;
using UnityEngine;

public class EndWaterLevel : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        player.MoveToScene(0);
    }
}