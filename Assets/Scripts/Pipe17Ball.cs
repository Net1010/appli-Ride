using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe17Ball : MonoBehaviour
{
    public float lifetime;
    public float speed;
    private float startPos;
    void Start()
    {
        startPos = transform.position.x;
    }

    public void FixedUpdate()
    {
        float distance = Mathf.Abs(startPos - transform.position.x);
        if (distance > 348) { Destroy(gameObject); }
        transform.Translate(Vector3.left * speed);
    }

}
