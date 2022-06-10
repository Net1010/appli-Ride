using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    static readonly int materialWidthId = Shader.PropertyToID("_Width");
    static readonly int materialHeightId = Shader.PropertyToID("_Height");
    static readonly int materialResRatioId = Shader.PropertyToID("_ResolutionRatio");

    public static Color SetColor(Color targetcolor, float alpha)
    {
        targetcolor.a = Mathf.Clamp(alpha, 0f, 1f);
        return targetcolor;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void DuplicateMat(Image img)
    {
        var mat = GameObject.Instantiate(img.material);
        img.material = mat;
    }

    // adjusts material's aspect ratio for patterns
    public static Vector2 AdjustMatRatio(Material mat, float _width, float _height)
    {
        mat.SetFloat(materialResRatioId, Mathf.Max(_width, 1) / Mathf.Max(_height, 1));
        return new Vector2(_width, _height);
    }

    // duplicates item as after effect then pops out and fades
    public static void AfterImage(RectTransform rtImage, float duration)
    {
        RectTransform imageDup = GameObject.Instantiate(rtImage, rtImage.transform.parent);                   
        LeanTween.scale(imageDup, new Vector3(2, 2, 0), duration);
        LeanTween.alpha(imageDup, 0f, duration).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
            GameObject.Destroy(imageDup.gameObject);
        }
        );
    }

}