using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : SManager
{
    [SerializeField]
    private GameObject ui_canvas;
    [SerializeField]
    private Image pDotSq;
    [SerializeField]
    private RectTransform bgFade;

    [SerializeField]
    private Sprite transitionBoxSprite;
    readonly private float transitionDuration = 0.4f;

    readonly private float RotateSpeed = 10f;

    [SerializeField]
    UI_MagneticInfiniteScroll scrollRect;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    private void FixedUpdate()
    {
        pDotSq.transform.Rotate(Vector3.forward, Time.deltaTime * RotateSpeed);
    }
    private GameObject CreateTransitionBox(RectTransform iconRt, Color transitionBoxCol)
    {
        GameObject transitionBox = new GameObject("transitionBox");
        transitionBox.transform.SetParent(ui_canvas.transform);
        transitionBox.transform.position = iconRt.transform.position;
        RectTransform boxRtrans = transitionBox.AddComponent<RectTransform>();
        boxRtrans.sizeDelta = new Vector2(10, 10);

        Image boxImage = transitionBox.AddComponent<Image>();
        boxImage.sprite = transitionBoxSprite;
        boxImage.GetComponent<Image>().color = transitionBoxCol;
        return transitionBox;
    }

    // when icon is clicked, transitions out to different scene
    public void OnTransitionOut(GameObject icon)
    {
        interactable = false; 
        Color transitionBoxCol = new Color32(0xFF, 0xFF, 0xFF, 0xFF); ;
        var iconLabel = icon.name.Split(char.Parse("_"))[0];                                                    // because items in the content of the scroll rect are duplicated, the dupe names have _2 on them. get the original name by splitting string
        switch (iconLabel)
        {
            case "Campaign":
                transitionBoxCol = new Color32(0xe4, 0xae, 0xca, 0xFF);
                break;
            case "Endless":
                transitionBoxCol = new Color32(0x15, 0x6e, 0x8b, 0xFF);
                break;
            case "Credits":
                transitionBoxCol = new Color32(0x65, 0xb8, 0x7f, 0xFF);
                break;
            default:
                return;
        }
        GameObject boxRtrans = CreateTransitionBox(scrollRect.GetPivot(), transitionBoxCol);
        LeanTween.scale(boxRtrans, new Vector2(Screen.width/10 * 2, Screen.height/10 * 2), transitionDuration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() => {
                var sceneManager = SceneCtrl.Instance;
                sceneManager.transitionColor = transitionBoxCol;
                sceneManager.LoadScene(iconLabel);
            });
    }
    public override void OnTransitionIn()
    {
        base.OnTransitionIn();
        
        var sceneManager = SceneCtrl.Instance;

        // for coming from campaign, endless or credits
        if (sceneManager.prevScene != "Intro") { CEOnTransitionIn(); }

        // for coming from intro
        else {
            // creates a box and fits it to the screen. gets rid of the sprite set in CreateTransitionBox
            var transitionBox = CreateTransitionBox(scrollRect.GetPivot(), new Color32(0xef, 0xef, 0xef, 0xFF));
            var boxRtrans = transitionBox.GetComponent<RectTransform>();
            boxRtrans.localPosition = new Vector3(0, 0, 0);
            boxRtrans.GetComponent<Image>().sprite = null;
            boxRtrans.anchorMin = new Vector2(0, 0);
            boxRtrans.anchorMax = new Vector2(1, 1);
            StartCoroutine(IntroWaitForScrollRect(boxRtrans));
        }
    }

    // for coming from campaign, endless or credits
    // creates an empty icon sprite box that shrinks in
    public void CEOnTransitionIn()
    {
        var sceneManager = SceneCtrl.Instance;
        var transitionBoxCol = sceneManager.transitionColor;
        GameObject boxRtrans = CreateTransitionBox(scrollRect.GetPivot(), transitionBoxCol);
        boxRtrans.GetComponent<RectTransform>().localScale = new Vector2(Screen.width / 10 * 2, Screen.height / 10 * 2);
        StartCoroutine(CEWaitForScrollRect(boxRtrans));
    }

    // waits for scroll rect to finish setting up before transitioning in
    // shrinks sprite box in
    private IEnumerator CEWaitForScrollRect(GameObject transitionBox)
    {
        while (!scrollRect.setupComplete) { yield return null; }
        LeanTween.scale(transitionBox, new Vector3(0, 0), transitionDuration)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() => {
                Destroy(transitionBox);
                interactable = true;
            });
    }

    // from intro to main menu
    // fades bg
    private IEnumerator IntroWaitForScrollRect(RectTransform boxRtrans)
    {
        while (!scrollRect.setupComplete) { yield return null; }
        LeanTween.alpha(boxRtrans, 0f, 1f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(() => {
                Destroy(boxRtrans.gameObject);
                interactable = true;
            });
    }
}
