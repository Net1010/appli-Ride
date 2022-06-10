using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionSpawner : MonoBehaviour
{
    public GameObject LevelSection;
    public GameObject[] pipeArray;
    public List<GameObject> sections = new List<GameObject>();
    public Queue<float> endPoints = new Queue<float>();
    private LevelData levelData;
    private bool campaign;
    private int campaignIndex;
    private GameObject goal;
    private GameObject pipeWarpFlip;
    private int endlessCounter;
    private float warpChance;

    private void AddSection()
    {
        GameObject lastSection = sections[sections.Count - 1];

        //makes sure the new pipe starts where the last pipe ends
        Vector3 lastSecPos = lastSection.transform.GetChild(0).transform.position;
        float meshSize = lastSection.GetComponentsInChildren<MeshCollider>()[0].bounds.size.x;
        endPoints.Enqueue(lastSecPos.x + meshSize);
        Vector3 nextPosition = new Vector3(lastSecPos.x + meshSize, lastSecPos.y, lastSecPos.z);
        sections.Add(Instantiate(LevelSection, nextPosition, Quaternion.Euler(lastSection.transform.GetChild(0).transform.eulerAngles.x, 0, 0)));
        //makes sure the rotation of the cylinder child is equal to the previous section
        sections[sections.Count - 1].transform.GetChild(0).transform.rotation = lastSection.transform.GetChild(0).transform.rotation;
    }

    private void GetCampaignSection()
    {
        if (campaignIndex == pipeArray.Length) { LevelSection = goal; }
        else { 
            LevelSection = pipeArray[campaignIndex];
            campaignIndex++;
        }
    }
    private void GetRandomSection() {
        if (endlessCounter > 5 & Random.value < warpChance)
        {
            LevelSection = pipeWarpFlip;
            endlessCounter = 0;
            warpChance = 0;
        }
        else
        {
            warpChance += 0.04f;
            endlessCounter++;
            LevelSection = pipeArray[Random.Range(0, pipeArray.Length)];
        }
    }

    public void SpawnThree()
    {
        if (sections.Count <= 2) {
            for (int i = 0; i < 3; i++)
            {
                if (campaign) { GetCampaignSection(); } 
                else { GetRandomSection(); }
                AddSection();
            }
        }
    }

    public void RemoveFirst()
    {
        endPoints.Dequeue();
        Destroy(sections[0]);
        sections.RemoveAt(0);
        if (sections.Count < 3)
        {
            SpawnThree();
        }
    }
    private void EndlessSetup()
    {
        pipeWarpFlip = (GameObject)Resources.Load("Prefabs/Base/PipeWarpFlip");
        endlessCounter = 0;
        warpChance = 0;
        pipeArray = Resources.LoadAll<GameObject>("Prefabs/" + levelData.endlessMode);
        SpawnThree();
    }

    void CampaignSetup(string[] campaignPipes)
    {
        pipeArray = new GameObject[campaignPipes.Length];
        for (int i = 0; i < campaignPipes.Length; i++)
        {
            pipeArray[i] = (GameObject)Resources.Load("Prefabs/" + campaignPipes[i]);
        }
        SpawnThree();
    }
    void Start()
    {

        LevelSection = (GameObject)Resources.Load("Prefabs/Base/PipeBase");
        sections.Add(Instantiate(LevelSection, Vector3.zero, Quaternion.Euler(0, 0, 0)));

        levelData = LevelData.instance;
        campaign = levelData.campaign;
        //campaign = true;
        if (campaign) {
            goal = (GameObject)Resources.Load("Prefabs/Base/Goal");
            campaignIndex = 0;
            campaign = true;
            CampaignSetup(levelData.stageList);
            //string[] temp = { "Normal/1", "Normal/5", "Normal/6", "Normal/1", "StageOnly/H2-Stage" };
            //CampaignSetup(temp);
        }
        else
        {
            EndlessSetup();
        }
    }
}
