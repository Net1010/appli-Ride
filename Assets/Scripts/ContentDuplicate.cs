using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ContentDuplicate : MonoBehaviour
{
    [SerializeField]
    private RectTransform content = null;
    [SerializeField]
    private UI_InfiniteScroll scrollRect;

    void Awake()
    {
        if (content != null)
        {
            var childCount = content.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var icon = content.GetChild(i);
                var dupeIcon = GameObject.Instantiate(content.GetChild(i), content);
                dupeIcon.name = icon.name + "_2";                            // changes name to icon2, for main menu manager to remove last character 
            }
            scrollRect.Init();                                              // setup infinite scroll after duplicating content
            content.GetComponent<ContentSizeScaler>().Init();               // make each item have button
        }

    }
}
