using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform  player;
    [SerializeField] float      distance = -10f;

    void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, distance);
    }
}
