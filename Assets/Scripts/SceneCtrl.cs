using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    private LevelData levelData;
    private AudioManager audioManager;
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    public Color transitionColor = Color.white;
    public Material transitionMaterial;

    public string prevScene;
    public static SceneCtrl Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null & Instance != this) { Destroy(gameObject); }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnEnable()
    {
        transitionColor = Color.white;
        SceneManager.sceneLoaded += OnSceneLoaded;
        prevScene = "Intro";
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelData = LevelData.instance;
        audioManager = AudioManager.instance;
        //makes sure audio persists if the player clicks replay or retry
        //if (scene.name == "Game" & levelData.prevScene != "Game") { audioManager.Play(levelData.bgm, true); }
        if (scene.name == "Game" & prevScene != "Game") { audioManager.Play(levelData.bgm, true); }
    }
    public void SelectStage(DomainData domainData)
    {
        levelData.campaignStage = domainData.campaignStage;
        //GenericSetup(true);
        levelData.speed = domainData.speed;
        levelData.health = domainData.health;
        levelData.up = domainData.up;
        levelData.stageList = domainData.stageList;
        levelData.bgm = domainData.bgm;
        levelData.domainData = domainData;
        levelData.campaign = true;
        LoadScene("Game");
    }

    public void GenericSetup(bool campaign)
    {
        levelData.speed = 2f;
        levelData.health = 3;
        levelData.up = false;
        levelData.campaign = campaign;
    }

    public void EndlessSetup(string mode)
    {
        levelData.campaignStage = "";
        GenericSetup(false);
        if (mode == "Hard") { levelData.speed = 3.5f; }
        levelData.bgm = mode == "Hard" ? "TheLongestYear" : "TheLongestYearShort";
        levelData.endlessMode = mode;
        LoadScene("Game");
    }
    public void LoadScene(string sceneName)
    {
        //determine mode from replay or retry in game scene
        if (sceneName == "FromGame") {
            FindObjectOfType<PauseResume>().ResumeGame();
            audioManager.Endtrack();
            sceneName = GameObject.FindObjectOfType<LevelData>().campaign ? "Campaign" : "Endless"; 
        }
        //makes sure pause reset will reset bgmvol
        else if (SceneManager.GetActiveScene().name == "Game" & sceneName == "Game") {
            audioManager.bgm.volume = audioManager.bgmVol; }

        prevScene = SceneManager.GetActiveScene().name;

        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName));
        StartCoroutine(LoadingScene());
    }

    private IEnumerator LoadingScene()
    {
        for (int i=0; i < scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone) {
                yield return null; }
        }
        scenesToLoad.Clear();
        FindObjectOfType<SManager>().OnTransitionIn();
    }

}
