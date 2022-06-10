using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scales content item
// requires UI_MagneticInfiniteScroll
public class ContentItemScaletoPivot : MonoBehaviour
{
    [Tooltip("The pointer to the pivot, the visual element for centering objects.")]
    [SerializeField]
    private RectTransform pivot = null;
    [SerializeField]
    private UI_MagneticInfiniteScroll scrollRect;

    [SerializeField]
    private bool _isVertical = false;

    private RectTransform item = null;
    private float distance = 0;
    public float initScale = 1;
    public float scaleRatio;

    // Start is called before the first frame update
    private void Awake()
    {
        item = GetComponent<RectTransform>();
        initScale = item.localScale.x;
    }

    // Update is called once per frame
    private void Update()
    {
        if (scrollRect._isMovement) { Scaleup(); } 
    }

    public void OnClick()
    {
        if (!SManager.instance.interactable) { return; }
        if (Mathf.Abs(item.localScale.x - initScale) < 0.05f)       // when clicking on an item at pivot, change scene (position not precise enough)
        {
            AudioManager.instance.PlayClick();
            UI_MagneticInfiniteScroll.SetPivotIndex(scrollRect.FindChildIndex(item));
            FindObjectOfType<MainMenuManager>().OnTransitionOut(item.gameObject);
        }
        else {                                                      // when clicking on an item not on pivot, make that item go to pivot
            AudioManager.instance.Play("ModeCycle");
            int index = scrollRect.FindChildIndex(item);            
            scrollRect.FinishPrepareMovement();
            scrollRect.GoToPivot(index);
        }
    }


    public void SetupChild(RectTransform _pivot, UI_MagneticInfiniteScroll _scrollRect)
    {
        pivot = _pivot;
        scrollRect = _scrollRect;
    }
    public void Scaleup()
    {
        if (pivot != null)
        {
            distance = Mathf.Abs(GetRightAxis(item.position) - GetRightAxis(pivot.position));
            scaleRatio = Mathf.Pow(0.4f, distance * 0.005f);
            var newScale = scaleRatio * initScale;
            item.localScale = new Vector3(newScale, newScale, newScale);
        }
        else { Debug.LogWarning("No Pivot"); }
    }
    private float GetRightAxis(Vector2 vector) { return _isVertical ? vector.y : vector.x; }
    public float GetScaleRatio() { return scaleRatio; }
}
