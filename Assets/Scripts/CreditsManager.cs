using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : SManager
{
    [SerializeField]
    private Image bgTransitionIn;
    [SerializeField]
    private Material bgFromMainMenuMat;

    [SerializeField]
    private GameObject ui_canvas;
    private GameObject iconTouch = null;
    private GameObject iconSelected = null;
    private UICustomClick clickManager;
    private string UIButtonTag;

    [SerializeField]
    private GameObject backBar;


    static readonly int materialUseMultId = Shader.PropertyToID("_UseMult");
    private void Start()
    {
        clickManager = new UICustomClick();
        clickManager.Init(ui_canvas);
        UIButtonTag = "UI Custom Button";

        Utilities.DuplicateMat(backBar.transform.GetChild(0).GetComponent<Image>());
    }
    protected override void Update()
    {
        base.Update();
        if (Input.touchCount > 0 && interactable)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                iconTouch = clickManager.TouchPhaseBegin(UIButtonTag);
                if (iconTouch != null) { iconTouch.transform.GetChild(0).GetComponent<Image>().material.SetFloat(materialUseMultId, 1); }                 // when an object is clicked it will replicate dark multiplied color like a button
            }
            else if (iconTouch != null && Input.GetTouch(0).phase == TouchPhase.Ended) { TouchEndPhase(); }
        }
        void TouchEndPhase()
        {
            GameObject uiElement = clickManager.UIClick(UIButtonTag);
            iconTouch.transform.GetChild(0).GetComponent<Image>().material.SetFloat(materialUseMultId, 0);
            if (iconTouch == uiElement)
            {                    // only activates if the first icon touched is the same as when the user lets go
                switch (uiElement.name)
                {
                    case "Back Button":
                        audioManager.PlayClick();
                        CEOnTransitionOut(bgTransitionIn);
                        break;
                }
            }
            iconTouch = null;
        }
    }
    public override void OnTransitionIn()
    {
        base.OnTransitionIn();
        MatOnTransitionIn(bgTransitionIn, bgFromMainMenuMat, 0f, 1f, 0.5f);
    }
}
