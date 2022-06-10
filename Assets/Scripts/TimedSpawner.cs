using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ObjectSpawn;

    [SerializeField]
    private float spawnTime;
    void Start()
    {
        InvokeRepeating("SpawnObject", 0f, spawnTime);
    }

    void SpawnObject()
    {
        GameObject obj = Instantiate(ObjectSpawn, transform.position, Quaternion.Euler(transform.eulerAngles));
        obj.transform.SetParent(this.transform.parent);
    }
}
