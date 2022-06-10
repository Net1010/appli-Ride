using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public CanvasGroup gameoverScreen;
    public TextMeshProUGUI pointsText;
    public float Duration = 2f;

    void Start()
    {
        gameoverScreen.alpha = 0f;
        gameoverScreen.gameObject.SetActive(false);
    }
    public void Setup(int score, bool campaign)
    {
        gameoverScreen.gameObject.SetActive(true);
        if (campaign) { pointsText.gameObject.SetActive(false);  }
        else { pointsText.text = score.ToString() + " Points"; }
        StartCoroutine(FadeIn(Duration));
    }

    private IEnumerator FadeIn(float duration)
    {
        float i = 0f;
        float rate = 1f / duration;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            gameoverScreen.alpha = Mathf.Lerp(0f, 1f, i);
            yield return null;
        }
    }
}
