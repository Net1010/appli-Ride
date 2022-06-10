using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelIcon : MonoBehaviour
{
    static private Sprite selectedSprite;
    static private Sprite unSelectedSprite;

    [SerializeField]
    private DomainData levelData;

    private Text domainText;
    private Image iconImage;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "UI Custom Button";                                                                            // for custom button (UICustomClick)
        if (selectedSprite == null) { selectedSprite = Resources.Load<Sprite>("Materials/domain_icon_selected"); }
        if (unSelectedSprite == null) { unSelectedSprite = Resources.Load<Sprite>("Materials/domain_icon"); }

        domainText = GetComponentInChildren<Text>();
        domainText.text = levelData.campaignStage;

        iconImage = transform.GetChild(0).GetComponent<Image>();
        Utilities.DuplicateMat(iconImage);
    }
    public void Select() {
        iconImage.sprite = selectedSprite;
    }
    public void DeSelect() {
        iconImage.sprite = unSelectedSprite;
    }

    public string GetName() {
        return domainText.text; }
    public string GetDomainName() { return levelData.domainName; }
    public int GetPassword() {
        int pass = 0;
        string txt = domainText.text;
        for (int i = 0; i < txt.Length; i++) { pass += txt[i]; }
        pass %= 11;
        if (pass <= 4) { pass += 5; }
        return pass;
    }
    public DomainData GetDomainData() { return levelData; }
}
