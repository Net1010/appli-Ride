using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTagRemove : MonoBehaviour
{
    private string tagHold;
    private float waitTime;
    private Color matColor;
    void Start()
    {
        matColor = this.GetComponent<Renderer>().material.color;
        this.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
        tagHold = this.tag;
        this.tag = "Untagged";
        waitTime = 0.6f;
        StartCoroutine(TagWait());
    }
    private IEnumerator TagWait()
    {
        float i = 0f;
        float rate = 1f / waitTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            this.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, matColor, i);
            yield return null;
        }
        this.tag = tagHold;
    }
}
