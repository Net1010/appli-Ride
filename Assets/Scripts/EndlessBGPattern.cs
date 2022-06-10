using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessBGPattern : MonoBehaviour
{
    private Material bgMat;

    static readonly int materialSplitValId = Shader.PropertyToID("_SplitVal");
    static readonly int materialPDotMoveAmtId = Shader.PropertyToID("_PdotMoveAmt");
    static readonly int materialStripeMoveAmtId = Shader.PropertyToID("_StripeMoveAmt");

    // Start is called before the first frame update
    void Start() {
        bgMat = Instantiate(transform.GetComponent<Image>().material);                                    // duplicates mat for runtime so it doesn't override the original
        this.GetComponent<Image>().material = bgMat;
    }
    // Update is called once per frame
    private void Update()
    {
    }

    public void AdjustBGSplit(float initial, float target, float duration, bool moveStripes) {
        var currSplitVal = bgMat.GetFloat(materialSplitValId);
        LeanTween.value(gameObject, initial, target, duration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnUpdate((float value) => { 
                bgMat.SetFloat(materialSplitValId, value);
            })
            .setOnComplete(() => { bgMat.SetFloat(materialSplitValId, target); ; });

        float t = 0.5f * 30f;
        var initSpd = moveStripes ? 0.5f : -0.5f;
        var finalSpd = moveStripes ? t : -t;
        var matSpdId = moveStripes ? materialStripeMoveAmtId : materialPDotMoveAmtId;
        LeanTween.value(gameObject, initSpd, finalSpd, duration * 3f)
            .setEase(LeanTweenType.easeInQuad)
            .setOnUpdate((float moveVal) => {
                bgMat.SetFloat(matSpdId, moveVal); });
        LeanTween.alpha(gameObject.GetComponent<RectTransform>(), 0f, 2.5f);
    }
    private void SpeedPdot()
    {

    }
}
