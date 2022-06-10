using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseResume : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject PauseButton;
    public bool GamePaused;
    public GameObject controlLayout;
    private bool inTutStage;

    private AudioManager audioManager;
    private LevelData levelData;

    void Start()
    {
        GamePaused = false;
        audioManager = AudioManager.instance;
        levelData = LevelData.instance;
        if (levelData.campaignStage == "Tut1" || levelData.campaignStage == "Tut2") 
        {
            inTutStage = true;
            controlLayout.SetActive(true); 
        }
        else 
        {
            inTutStage = false;
            controlLayout.SetActive(false); 
        }
    }

    void Update()
    {
        if (GamePaused) { Time.timeScale = 0; }
        else { Time.timeScale = 1; }
    }
    public void RemoveControlLayout()
    {
        if (levelData.campaignStage == "Tut1" || levelData.campaignStage == "Tut2") { controlLayout.SetActive(false); }
    }

    public void PauseGame()
    {
        if (inTutStage) { controlLayout.SetActive(false); }
        GamePaused = true;
        PauseScreen.SetActive(true);
        PauseButton.SetActive(false);
        audioManager.PauseBgmVol();
    }

    public void ResumeGame()
    {
        if (inTutStage) { controlLayout.SetActive(true); }
        audioManager.PauseBgmVol();
        GamePaused = false;
        PauseScreen.SetActive(false);
        PauseButton.SetActive(true);
    }
}
