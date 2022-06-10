using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuData : MonoBehaviour
{
    public int mode;
    public static MenuData Instance { get; private set; }
    void Awake()
    {
        if (Instance != null & Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }
}
