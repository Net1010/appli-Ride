using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentSizeScaler : MonoBehaviour
{
    [SerializeField]
    private UI_MagneticInfiniteScroll _scrollRect;
    [SerializeField]
    private RectTransform pivot = null;

    private bool setupComplete = false;

    public void Init()                                                  // adds button and onclick to each content item
    {
        foreach (Transform child in transform)
        {
            child.gameObject.AddComponent<ContentItemScaletoPivot>().SetupChild(pivot, _scrollRect);
            child.gameObject.AddComponent<Button>().onClick.AddListener(delegate { child.gameObject.GetComponent<ContentItemScaletoPivot>().OnClick(); });
            //child.gameObject.GetComponent<ContentItemScaletoPivot>().OnClick();
        }
        StartCoroutine(ScrollWait());
    }
    public void ScaleChildren()
    {
        foreach (Transform child in transform) {
            child.gameObject.GetComponent<ContentItemScaletoPivot>().Scaleup();
        }
    }
    public bool IsComplete() { return setupComplete; }
    IEnumerator ScrollWait()
    {
        yield return new WaitUntil(() => _scrollRect.setupComplete);
        ScaleChildren();
        setupComplete = true;
    }
}
