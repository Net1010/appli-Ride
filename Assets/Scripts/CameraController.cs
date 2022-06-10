using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject playerObj;
    public PlayerMovement player;
    private float xOffset = -8, yOffset = 4;
    private bool isMoving;
    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        playerObj = player.gameObject;
        isMoving = false;
    }
    void Update()
    {
        transform.position = playerObj.transform.position + new Vector3(xOffset, yOffset, 0f);
        transform.LookAt(playerObj.transform.position);
        transform.position = playerObj.transform.position + new Vector3(-7f, 6f, 0f);
    }
}
