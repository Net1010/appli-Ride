using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScreen : MonoBehaviour
{
    public CanvasGroup goalScreen;
    public float Duration = 2f;

    void Start()
    {
        goalScreen.alpha = 0f;
        goalScreen.gameObject.SetActive(false);
    }
    public void Setup()
    {
        goalScreen.gameObject.SetActive(true);
        StartCoroutine(fadeIn(Duration));
    }

    private IEnumerator fadeIn(float duration)
    {
        float i = 0f;
        float rate = 1f / duration;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            goalScreen.alpha = Mathf.Lerp(0f, 1f, i);
            yield return null;
        }
    }
}
