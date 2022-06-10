using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_MagneticInfiniteScroll : UI_InfiniteScroll, IDragHandler, IEndDragHandler
{
    public event Action<GameObject> OnNewSelect;


    [Tooltip("The pointer to the pivot, the visual element for centering objects.")]
    [SerializeField]
    private RectTransform pivot = null;
    [Tooltip("The pointer to the object container")]
    [SerializeField]
    private RectTransform content = null;
    [Tooltip("the maximum speed that allows you to activate the magnet to center on the pivot")]
    [SerializeField]
    private float maxSpeedForMagnetic;
    [SerializeField]
    [Tooltip("The initial index of the object which must be initially centered")]
    static private int indexStart = 1;
    [SerializeField]
    [Tooltip("The time to decelerate and aim for the pivot")]
    private float timeForDeceleration = 0.05f;


    [SerializeField]
    private ScrollRect scrollRect = null;
    public bool setupComplete = false;

    private float _pastPosition = 0;

    private float _currentSpeed = 0.0f;
    private float _stopValue = 0.0f;
    private readonly float _waitForContentSet = 0.1f;
    private float _currentTime = 0;
    //private int nearestIndex = 0;

    private bool _useMagnetic = true;
    private bool _isStopping = false;
    public bool _isMovement = false;

    private float initPosX = 0f;                                                            // for lerp in stopping

    private bool stopEnd;                                                                   // to play stopping in fixed update (isStopping checks in update for deceleration purposes, stopEnd checks in fixedupdate for animation purposes, to look smooth)
    public List<RectTransform> Items { get; }

    [SerializeField]
    [Range(0, 10)]
    private int childNum;

    protected override void Awake()
    {
        maxSpeedForMagnetic = 10f;
        childNum = 5;
        timeForDeceleration = 0.05f;
        stopEnd = false;
        scrollRect.transform.SetAsFirstSibling();

        base.Awake();
        StartCoroutine(SetInitContent());
    }

    private void Update()
    {
        // update only after dragging but before movement stops
        if (!content || !pivot || !_useMagnetic || !_isMovement || items == null || scrollRect == null) { return; }
        // Allows the scroll to continue until it reaches its max speed (speed measured in distance from previous pos to current pos, will decelerate until it is less than max speed)
        float currentPosition = content.anchoredPosition.x;
        _currentSpeed = Mathf.Abs(currentPosition - _pastPosition);
        _pastPosition = currentPosition;
        if (Mathf.Abs(_currentSpeed) > maxSpeedForMagnetic) { return; }

        if (_isStopping) {                                                                     // moves content anchor to place nearest item into pivot using lerp
            stopEnd = true;
            //Stopping();
        }
        else {                                                                                 // when about to stop, obtain the index of the item closest to the pivot to snap to
            int nearestIndex = FindNearestItem();
            GoToPivot(nearestIndex);
            stopEnd = false;
        }
    }
    private void FixedUpdate()
    {
        if (stopEnd)
        {
            Stopping();
        }
    }
    public static void SetPivotIndex(int index) { indexStart = index; }
    public RectTransform GetPivot() { return pivot; }
        
    private void Stopping()                                                                      // when stopping to place item at pivot
    {
        Vector2 anchoredPosition = content.anchoredPosition;
        if (_currentTime <= timeForDeceleration)
        {
            float valueLerp = _currentTime / timeForDeceleration;
            float newPosition = Mathf.Lerp(initPosX, _stopValue, valueLerp);                    
            content.anchoredPosition = _isVertical ? new Vector2(anchoredPosition.x, newPosition) :
                        new Vector2(newPosition, anchoredPosition.y);
            _currentTime += Time.deltaTime;
        }
        else if (_currentTime <= timeForDeceleration + 0.2f)
        {
            content.anchoredPosition = _isVertical ? new Vector2(anchoredPosition.x, _stopValue) :
                        new Vector2(_stopValue, anchoredPosition.y);
            _currentTime += Time.deltaTime;
        }
        else if (_currentTime <= timeForDeceleration + 0.5f)
        {
            _currentTime += Time.deltaTime;
        }
        else
        {
            _isStopping = false;
            _isMovement = false;
            stopEnd = false;
        }
    }

    public void GoToPivot(int index) {                                                      // forces content to stop and move item at index to pivot
        _isStopping = true;
        initPosX = content.anchoredPosition.x;                                  // initial position x for lerp in stopping
        _stopValue = GetAnchoredPositionForPivot(index);
        scrollRect.StopMovement();
    }
    public int FindChildIndex(RectTransform item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item) { return i;  }
        }
        return -1;
    }

    private int FindNearestItem() {
        int nearestIndex = 0;
        var closestDistance = Mathf.Infinity;                                               // closest distance to pivot
        for (int i = 0; i < items.Count; i++)                                               // looks through each item to find the nearest item to pivot
        {
            var item = items[i];
            if (item == null) { continue; }                                                            // moves to next item if item is null
            var aux = Mathf.Abs(item.position.x - pivot.position.x);           // length between pivot and item
            if (aux < closestDistance)
            {
                closestDistance = aux;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    public void OnDrag(PointerEventData data) {                                 
        _isMovement = true;
        _useMagnetic = false;
        _isStopping = false;
        stopEnd = false;
    }

    public void OnEndDrag(PointerEventData data) { FinishPrepareMovement(); }

    public override void SetNewItems(ref List<Transform> newItems)
    {
        foreach (var element in newItems)
        {
            RectTransform rectTransform = element.GetComponent<RectTransform>();
            if (rectTransform && pivot)
            {
                rectTransform.sizeDelta = pivot.sizeDelta;
            }
        }
        base.SetNewItems(ref newItems);
    }

    public void SetContentInPivot(int index)
    {
        float newPos = GetAnchoredPositionForPivot(index);                      // gets distance of item from pivot
        Vector2 anchoredPosition = content.anchoredPosition;
        if (content) {                                                          // places item in the pivot location by shifting the entire content anchor point by newPos amount (e.g. pivot.x is 200, item.x is 300, newPos is -100, moves entire content -100 spaces to make item at pivot. The anchor of content is now not aligned with the anchor of pivot.)
            content.anchoredPosition = _isVertical ? new Vector2(anchoredPosition.x, newPos) : new Vector2(newPos, anchoredPosition.y);
            _pastPosition = content.anchoredPosition.x;
        }
    }

    // On awake, sets starting index to pivot
    private IEnumerator SetInitContent()
    {
        yield return new WaitForSeconds(_waitForContentSet);                    
        SetContentInPivot(indexStart);

        // scales content items by proximity to pivot
        yield return new WaitForSeconds(_waitForContentSet);

        scrollRect.transform.SetSiblingIndex(childNum);                                // ordered where the user sets it
        setupComplete = true;
    }
        
    // Pivot anchor must be on the top left of it's parent
    private float GetAnchoredPositionForPivot(int index)
    {
        if (!pivot || items == null || items.Count < 0) { return 0f; }

        index = Mathf.Clamp(index, 0, items.Count - 1);                         // makes sure the index isn't out of range

        //float posItem = GetRightAxis(items[index].anchoredPosition);            // gets anchored pos of item (x or y) (anchor to content which is the same as viewport)
        //float posPivot = GetRightAxis(pivot.anchoredPosition);                  // gets anchored pos of pivot (x or y) (anchor to viewport)
        float posItem = items[index].anchoredPosition.x;            
        float posPivot = pivot.anchoredPosition.x;              

        return posPivot - posItem;                                              // returns the distance of item from pivot
    }

    // when dragging is let go, prepare to stop
    public void FinishPrepareMovement()
    {
        _isMovement = true;                                                     // here as well as on drag because changing pivots can include touch, not just drag
        _useMagnetic = true;
        _isStopping = false;
        stopEnd = false;
        _currentTime = 0;                                                       // reset for lerp
    }

    private float GetRightAxis(Vector2 vector) {                                // returns the correct x or y pos
        return _isVertical ? vector.y : vector.x;
    }
}