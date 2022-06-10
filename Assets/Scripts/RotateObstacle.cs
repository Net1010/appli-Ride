using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    public float rotationDir;
    public float counterClockwise;
    public float randomMulti;
    void Awake()
    {
        if (counterClockwise == 0)
        {
            float[] pick = new float[] { -1, 1 };
            counterClockwise = pick[Random.Range(0, pick.Length)];
        }
        rotationDir = 1f * counterClockwise * randomMulti;
    }

    public void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationDir, 0f, 0f), Space.World);
    }
}
