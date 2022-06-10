using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe17Dir : MonoBehaviour
{
    public bool invert;
    public bool randomDir;
    void Start()
    {
        if (randomDir) { invert = Random.value >= 0.5; }
        if (invert) { transform.localScale = new Vector3(-1, 1, 1); }
    }

}
