using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMovement : MonoBehaviour
{
    public PlayerMovement player;

    private float xTilt;
    private Vector3 rotationDir;
    private float xOffset;

    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        //gets sensitivty offset
        xOffset = PlayerPrefs.GetFloat("sensitivityOffset", 0);
    }
    void Update()
    {
        ProcessInputs();
    }

    public void FixedUpdate()
    {
        if (player.health < 0) { return; }
        CylMove();
    }

    private void CylMove()
    {
        rotationDir = new Vector3(xTilt, 0f, 0f);
        transform.Rotate(rotationDir, Space.World);
    }

    private void ProcessInputs()
    {
        xTilt = Input.acceleration.x * (-4.5f - xOffset);
        if (xTilt > 5) { xTilt = 5f; }
        else if (xTilt < -5) { xTilt = -5f; }

        //if (Mathf.Abs(xTilt) < 0.009)
        //{
            //Debug.Log(xTilt);
            //xTilt *= 0f;
        //}

        if (player.transform.position.y > 14) { xTilt *= -1; }
    }
}
