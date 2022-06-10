using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampaignManager : SManager
{
    [SerializeField]
    private Image bgTransitionIn;
    [SerializeField]
    private Material bgFromMainMenuMat;
    static readonly int materialCtrlId = Shader.PropertyToID("_Ctrl");
    static readonly int materialStepCtrlId = Shader.PropertyToID("_StepCtrl");

    // for clicking ui buttons
    public GameObject ui_canvas;
    private GameObject iconTouch = null;
    private GameObject iconSelected = null;

    private UICustomClick clickManager;
    private string UIButtonTag;

    // buttons
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private GameObject backBar;
    [SerializeField]
    private GameObject settingsButton;
    [SerializeField]
    private GameObject enterButton;
    [SerializeField]
    private GameObject scrollRectButton;

    [SerializeField]
    private Text domainText;

    [SerializeField]
    private Text passwordText;

    private int currI;
    private int newTxtCount;
    readonly private int txtBuffer = 3;

    static readonly int materialUseMultId = Shader.PropertyToID("_UseMult");

    // for clicking enter
    [SerializeField]
    private GameObject internetSymbol;

    [SerializeField]
    private Settings settings;
    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        // for custom button clicks checking for the ui button tag
        clickManager = new UICustomClick();
        clickManager.Init(ui_canvas);
        UIButtonTag = "UI Custom Button";

        // duplicates mat for runtime so it doesn't override the original
        var backBarImg = backBar.transform.GetChild(0).GetComponent<Image>();
        var mat = Instantiate(backBarImg.material);                                               
        backBarImg.material = mat;

        Utilities.DuplicateMat(backBar.transform.GetChild(0).GetComponent<Image>());
        Utilities.DuplicateMat(settingsButton.transform.GetChild(0).GetComponent<Image>());
        Utilities.DuplicateMat(enterButton.transform.GetChild(0).GetComponent<Image>());
        Utilities.DuplicateMat(scrollRectButton.transform.GetChild(0).GetComponent<Image>());

        domainText.text = "";
        passwordText.text = "";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Input.touchCount > 0 && interactable) {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                iconTouch = clickManager.TouchPhaseBegin(UIButtonTag);
                if (iconTouch != null) { iconTouch.transform.GetChild(0).GetComponent<Image>().material.SetFloat(materialUseMultId, 1); }                 // when an object is clicked it will replicate dark multiplied color like a button
            }
            else if (iconTouch != null && Input.GetTouch(0).phase == TouchPhase.Ended) { TouchEndPhase(); }
        }
    }
    public override void OnTransitionIn() {
        base.OnTransitionIn();
        // no material transition when from game
        if (SceneCtrl.Instance.prevScene != "Game")
        {
            MatOnTransitionIn(bgTransitionIn, bgFromMainMenuMat, 0f, 1.5f, 0.75f);
        }
        // turn on interactable since it is off by default
        else { interactable = true; }
    }

    // updates text field when level is selected
    void UpdateFieldText()
    {
        var iconLI = iconTouch.GetComponent<LevelIcon>();
        iconLI.Select();                                                                                                                                // changes currently selected icon to having the pink sprite
        var name = iconLI.GetDomainName();
        var pass = iconLI.GetPassword();
        iconSelected = iconTouch;
        domainText.text = "";
        currI = -1;
        newTxtCount = 0;
        LeanTween.value(iconSelected, 0, name.Length * 3 + txtBuffer * 3, 1f)                                                                           // for domain text, replicating a similar style to mgr text flicker. txtBuffer * 3 to add aditional passes for when the passes will affix characters
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float i) => {
                if (currI == Mathf.FloorToInt(i)) { return; }                                                                                           // returns whenever value is not the next whole number (unfortunately the way it is implemented now it can potentially skip integers when flooring)
                currI = Mathf.FloorToInt(i);
                if (currI % 3 == 0) { newTxtCount += 1; }                                                                                               // new character added to the string every 3 passes
                domainText.text = new string('*', Mathf.Min(newTxtCount, name.Length));                                                                 // buffers a text with correct length
                var newTxt = "";
                string randSSelection = name.Replace(" ", "");                                                                                          // random characters only from the final name without space bars
                for (int j = 0; j < domainText.text.Length; j++)
                {                                                                                      // checks each character in the current domain text
                    if (j < newTxtCount - txtBuffer) { newTxt += name[j]; }                                                                             // every 3 passes (ideally without skipped integers in currI) will affix the correct character. minus txtBuffer meaning the amount of passes * 3 until it starts affixing chars
                    else { newTxt += randSSelection[Random.Range(0, randSSelection.Length)]; }                                                          // picks random character per pass to display to characters not affixed
                }
                domainText.text = newTxt;
            })
            .setOnComplete(() => { domainText.text = name; });
        LeanTween.value(iconSelected, 0, pass, 0.5f)                                                                                                    // for password text. simple display of * per pass
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float i) => {
                passwordText.text = new string('*', Mathf.FloorToInt(i));
            });
    }
    void TouchEndPhase() {
        GameObject uiElement = clickManager.UIClick(UIButtonTag);
        iconTouch.transform.GetChild(0).GetComponent<Image>().material.SetFloat(materialUseMultId, 0);
        if (iconTouch == uiElement) {                    // only activates if the first icon touched is the same as when the user lets go
            switch (uiElement.name) {
                case "Back Bar":
                    audioManager.PlayClick();
                    CEOnTransitionOut(bgTransitionIn);
                    break;
                case "Gear":
                    settings.OpenMenu();
                    break;
                case "Enter Button":
                    if (iconSelected == null) {
                        audioManager.Play("Reject");
                        return; 
                    }
                    //interactable = false;
                    audioManager.PlayClick();
                    var sceneManager = SceneCtrl.Instance;
                    sceneManager.SelectStage(iconSelected.GetComponent<LevelIcon>().GetDomainData());

                    // internet symbol after effect
                    /**
                    LeanTween.scaleX(internetSymbol, -internetSymbol.transform.localScale.x, 0.2f)
                        .setEase(LeanTweenType.easeInCubic)
                        .setOnComplete(() => {
                            internetSymbol.transform.localScale = new Vector3(1, 1, 1); 
                            Utilities.AfterImage(internetSymbol.GetComponent<RectTransform>(), 0.5f);
                        });
                    /**/
                    break;
                case "Scroll Rect Button":
                    ScrollToBottom();
                    break;
                default:                                // for level icon selected (if it was recorded in touch begin and it wasn't any of the previous ui elements, it must be level icon)
                    audioManager.Play("IconSelect");
                    LevelSelected();
                    break;
            }
        }
        iconTouch = null;
    }
    // scroll rect button, click to automatically move it
    void ScrollToBottom() {
        if (LeanTween.isTweening(gameObject)) { return; }                                                                                               // returns if it is currecntly scrolling automatically
        var initial = scrollRect.normalizedPosition.y;
        var target = 0f;
        if (scrollRect.normalizedPosition.y < 0.01f) {                                                                                                  // scrolls up if the scroll is at the bottom
            target = 1f; }
        LeanTween.value(gameObject, initial, target, 0.1f)                                                                                              // tween value of normalized scroll rect position
            .setEase(LeanTweenType.linear)
            .setOnUpdate((float yVal) => { scrollRect.normalizedPosition = new Vector2(0, yVal); })
            .setOnComplete(() => { scrollRect.normalizedPosition = new Vector2(0, target); });
    }
    void LevelSelected() {
        if (iconSelected != null) { iconSelected.GetComponent<LevelIcon>().DeSelect(); LeanTween.cancel(iconSelected); }                                // changes previously selected icon back to having the original blue sprite
        UpdateFieldText();
    }
}
