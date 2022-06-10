using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParticle : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + 100f, 0, 0);
    }
}
