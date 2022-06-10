using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSliceRotation : MonoBehaviour
{
    void Start()
    {
        this.transform.rotation = this.transform.parent.parent.GetChild(0).transform.rotation;
    }
}
