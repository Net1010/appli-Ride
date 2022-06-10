using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICustomClick
{
    private GraphicRaycaster ui_raycaster;
    private PointerEventData click_data;
    private List<RaycastResult> click_results;
    public void Init(GameObject ui_canvas)
    {
        ui_raycaster = ui_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
    }
    public GameObject TouchPhaseBegin(string tag) {
        var iconTouch = UIClick(tag);
        return iconTouch;
    }
    // assumes no overlap
    public GameObject UIClick(string tag) {
        click_data.position = Input.GetTouch(0).position;
        click_results.Clear();
        ui_raycaster.Raycast(click_data, click_results);

        foreach (RaycastResult result in click_results)
        {
            GameObject ui_element = result.gameObject;
            if (ui_element.CompareTag(tag)) { return ui_element; }
        }
        return null;
    }
}
