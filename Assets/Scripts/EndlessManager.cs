using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndlessManager : SManager
{
    [SerializeField]
    private EndlessBGPattern bgPattern;

    [SerializeField]
    private Image bgTransitionIn;
    [SerializeField]
    private Material bgFromMainMenuMat;

    // for clicking ui buttons
    public GameObject ui_canvas;
    private GameObject iconTouch = null;
    private GameObject modeTouch;                                                                                                                      // keeps track of which side was clicked

    private UICustomClick clickManager;
    private string UIButtonTag;

    [SerializeField]
    private GameObject backBar;
    [SerializeField]
    private GameObject settingsBar;
    [SerializeField]
    private GameObject normalImage;
    [SerializeField]
    private GameObject hardImage;

    static readonly int materialUseMultId = Shader.PropertyToID("_UseMult");

    [SerializeField]
    private Settings settings;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        clickManager = new UICustomClick();
        clickManager.Init(ui_canvas);
        UIButtonTag = "UI Custom Button";

        Utilities.DuplicateMat(backBar.transform.GetChild(0).GetComponent<Image>());
        Utilities.DuplicateMat(settingsBar.transform.GetChild(0).GetComponent<Image>());
        Utilities.DuplicateMat(normalImage.GetComponent<Image>());
        Utilities.DuplicateMat(hardImage.GetComponent<Image>());
    }
    protected override void Update() {
        base.Update();
        if (Input.touchCount > 0 && interactable)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) {                                                                                          // when touch begin phase
                iconTouch = clickManager.TouchPhaseBegin(UIButtonTag);
                if (iconTouch != null) { iconTouch.transform.GetChild(0).GetComponent<Image>().material.SetFloat(materialUseMultId, 1);}                // when an object is clicked it will replicate dark multiplied color like a button
                else { TouchBeginDiffMode();  }                                                                                                         // if no object is clicked in endless, it will select the difficulty mode of one side
            }
            else if ( Input.GetTouch(0).phase == TouchPhase.Ended) {                                                                                    // when touch end phase
                if (iconTouch != null) { TouchEndPhase(); }                                                                                             // when object was selected previously
                else if (modeTouch != null) { TouchEndDiffMode(); }                                                                                     // when no object was selected, select difficulty mode
            }
        }
    }
    public override void OnTransitionIn()
    {
        base.OnTransitionIn();
        if (SceneCtrl.Instance.prevScene != "Game")
        {
            MatOnTransitionIn(bgTransitionIn, bgFromMainMenuMat, 0f, 1.5f, 0.75f);
        }
        else { interactable = true; }
    }
    private void TouchBeginDiffMode() {
        modeTouch = SideClicked();
        modeTouch.GetComponent<Image>().material.SetFloat(materialUseMultId, 1);
    }
    private void TouchEndDiffMode() {
        if (modeTouch == SideClicked()) {
            interactable = false;
            var duration = 1f;
            if (modeTouch == normalImage) {
                OnModeSelect(0.5f, 1f, duration, false);
            }
            else {
                OnModeSelect(0.5f, 0f, duration, true);
            }
        }
        modeTouch.GetComponent<Image>().material.SetFloat(materialUseMultId, 0);
        modeTouch = null;
    }
    private void OnModeSelect(float initial, float target, float duration, bool moveStripes) {
        bgPattern.AdjustBGSplit(initial, target, duration, moveStripes);
        LeanTween.alpha(hardImage.GetComponent<RectTransform>(), 0f, duration * 0.5f);
        LeanTween.alpha(normalImage.GetComponent<RectTransform>(), 0f, duration * 0.5f);
        LeanTween.alpha(settingsBar.transform.GetChild(0).GetComponent<RectTransform>(), 0f, duration * 0.5f);
        LeanTween.alpha(backBar.transform.GetChild(0).GetComponent<RectTransform>(), 0f, duration * 0.5f);
        LeanTween.value(0f, 1f, 2.5f)
            .setOnComplete(() =>
            {
                string mode = moveStripes ? "Hard" : "Normal";
                SceneCtrl.Instance.EndlessSetup(mode);
            });
    }
    private GameObject SideClicked() {
        var touch = Input.touches[0];
        if (touch.position.x <= Screen.width / 2) { return normalImage; }
        return hardImage;
    }
    private void TouchEndPhase()
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
                case "Settings Button":
                    settings.OpenMenu();
                    break;
                default:
                    break;
            }
        }
        iconTouch = null;
    }
}
