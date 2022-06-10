using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe17Mat : MonoBehaviour
{
    private Pipe17Dir pipeScript;
    private RotateObstacle matScript;
    void Start()
    {
        pipeScript = GetComponentInChildren<Pipe17Dir>();
        matScript = GetComponentInChildren<RotateObstacle>();
        if (pipeScript.invert == (matScript.counterClockwise == 1))
        {
            matScript.rotationDir *= -1;
        }

    }

}
