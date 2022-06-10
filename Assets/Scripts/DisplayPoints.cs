using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPoints : MonoBehaviour
{
    public PlayerMovement player;
    public TextMeshProUGUI pointsText;
    public GameOverScreen gameOverUI;
    public float waitTime;
    private int score;
    private LevelData levelData;
    private bool campaign;

    void Start()
    {
        levelData = LevelData.instance;
        gameOverUI = GameObject.FindObjectOfType<GameOverScreen>();
        score = 0;
        waitTime = 0.7f;
        if (levelData == null || levelData.campaign) { 
            pointsText.gameObject.SetActive(false);
            campaign = true;
        }
        StartCoroutine(SpawnTimer());
    }
    private IEnumerator SpawnTimer()
    {
        while (player.health >= 0)
        {
            pointsText.text = (score).ToString() + " Points";
            yield return new WaitForSeconds(waitTime);
            score++;
        }
        gameOverUI.Setup(score, campaign);
    }
}
