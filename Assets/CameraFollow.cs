using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 distance;

    private void Update()
    {
        transform.position = player.position + distance;
        transform.LookAt(player);
    }
}
