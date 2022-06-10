using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{

    public string bgm;
    public bool campaign;
    public string[] stageList;
    public float speed;
    public int health;
    public bool up;
    public string prevScene;
    public string campaignStage;

    public string endlessMode;

    public DomainData domainData { get; set; }

    //public static LevelData instance;
    public static LevelData instance { get; private set; }
    void Awake()
    {
        if (instance != null & instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        GenericSetup();
    }
    public void GenericSetup()
    {
        speed = 2f;
        health = 3;
        up = false;
        campaign = true ;
        bgm = "TheLongestYearShort";
        endlessMode = "Hard";
    }
    
}
