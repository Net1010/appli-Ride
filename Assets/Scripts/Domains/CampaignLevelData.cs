using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage")]
public class DomainData : ScriptableObject
{
    public string campaignStage;
    public string bgm = "TheLongestYear";
    public string[] stageList;
    public float speed = 2f;
    public int health = 3;
    public bool up = false;
    public string domainName;
}

