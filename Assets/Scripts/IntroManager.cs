using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroManager : SManager
{
    [SerializeField]
    private RectTransform logoSymbol;
    [SerializeField]
    private RectTransform bgFade;
    [SerializeField]
    private TMP_Text text;

    protected override void Awake()
    {
        base.Awake();
        bgFade.gameObject.SetActive(true);
        bgFade.GetComponent<Image>().color = Color.black;
    }
    // Start is called before the first frame update
    void Start()
    {
        var textColor = text.color;
        textColor.a = 0;
        text.color = textColor;

        LeanTween.alpha(bgFade, 0f, 2f).setEase(LeanTweenType.linear).setOnComplete(BGBlackEnd);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.touchCount > 0 || Input.GetKey("space"))
        {
            if (!interactable) { return; }                                                                     // only activates once, player can't spam click
            interactable = false;

            LeanTween.cancel(bgFade);                                                                  // cancels curren tweening operations
            LeanTween.cancel(text.gameObject);

            text.text = "Start";                                                                               // changes text
            text.color = new Color(1, 1, 1, 1);

            TMP_Text textEffect = Instantiate(text, text.transform.parent);                                    // text duplicate pop out effect
            LeanTween.scale(textEffect.rectTransform, new Vector3(2, 2, 0), 0.5f);
            textEffect.LeanAlphaText(0f, 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => Destroy(textEffect.rectTransform.gameObject));

            var newColor = Color.white;
            newColor.a = 0f;
            bgFade.GetComponent<Image>().color = newColor;
            bgFade.gameObject.SetActive(true);
            LeanTween.alpha(bgFade, 1f, 2f).setEase(LeanTweenType.easeInCirc)
                .setOnComplete(() => {
                    SceneCtrl.Instance.LoadScene("Main Menu");
                }
            );
        }
    }
    void BGBlackEnd()
    {
        bgFade.gameObject.SetActive(false);
        bgFade.GetComponent<Image>().color = Color.white;
        TextFadeEnd(text);                                                                                      // starts text fade loop
    }
    void TextFadeStart(TMP_Text text) { text.LeanAlphaText(0f, 1.5f).setEase(LeanTweenType.linear).setOnComplete(() => TextFadeEnd(text)); }
    void TextFadeEnd(TMP_Text text) { text.LeanAlphaText(1f, 1.5f).setEase(LeanTweenType.linear).setOnComplete(() => TextFadeStart(text)); }

}
