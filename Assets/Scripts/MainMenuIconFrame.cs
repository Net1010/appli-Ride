using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuIconFrame : MonoBehaviour
{
    [SerializeField]
    private UI_MagneticInfiniteScroll scrollRect;
    [SerializeField]
    private Color farColor;
    [SerializeField]
    private Color closeColor;

    private ContentSizeScaler contentScaler;

    private ContentItemScaletoPivot itemContainer;
    private Image frameImage;

    static readonly int materialColorId = Shader.PropertyToID("_Color");

    // Start is called before the first frame update
    void Start()
    {
        frameImage = GetComponent<Image>();
        contentScaler = transform.parent.parent.GetComponent<ContentSizeScaler>();
        StartCoroutine(ScrollWait());
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollRect._isMovement) { ChangeColor(); }
    }
    void ChangeColor()
    {
        if (itemContainer == null) { itemContainer = transform.parent.gameObject.GetComponent<ContentItemScaletoPivot>(); }
        var colorRatio = Mathf.Clamp(Utilities.Remap(itemContainer.GetScaleRatio(), 0.9f, 1f, 0f, 1f), 0, 1);            
        //var colorRatio = Mathf.Clamp(itemContainer.getScaleRatio() * 2 - 1, 0, 1);                                                  // distance ratio formula item content (ContentItemScaletoPivot.cs), * 2 - 1 clamped to change frange from 0.5-1 to 0-1
        var newColor = Color.Lerp(farColor, closeColor, colorRatio);
        newColor.a = 1;
        frameImage.color = newColor;
    }
    IEnumerator ScrollWait() {
        yield return new WaitUntil(() => contentScaler.IsComplete());
        ChangeColor();
    }
}
