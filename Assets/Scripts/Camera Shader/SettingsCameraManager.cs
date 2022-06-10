using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsCameraManager : MonoBehaviour
{
    private SManager manager;
    // material settings for main camera's renderer to transition to settings menu
    [SerializeField]
    private Material mat;
    static readonly int materialBlendTexId = Shader.PropertyToID("_BlendTex");
    static readonly int materialColArrId = Shader.PropertyToID("_ColorArray");
    static readonly int materialDelayArrId = Shader.PropertyToID("_DelayArray");
    static readonly int materialCenterArrId = Shader.PropertyToID("_CenterArray");
    static readonly int materialTimeInitId = Shader.PropertyToID("_TimeInit");
    static readonly int materialDurationId = Shader.PropertyToID("_Duration");
    static readonly float duration = 1f;
    private float[] DelayArr = new float[3];                                    

    private Camera cam;

    private Vector2 res;                                                                                // resolution vector 2 of screen width and height
    private void Awake()
    {
        cam = GetComponent<Camera>();
        SetupTransition();
        
    }
    private void Start()                                                                                // start to make sure manager is initialized first
    {
        manager = SManager.instance;
    }
    public float ResetTransitionMat()
    {
        mat.SetFloat(materialTimeInitId, Time.time);              // resets shader timer (shader trigger)
        return duration + DelayArr[2];                                          // returns total time the animation will take (duration is the time for 1 circle's animation, delay is the time until the circle's animation start, it takes the last circle's delay + duration)
    }
    private void SetupTransition()
    {
        res = new Vector2(Screen.width, Screen.height);
        SetCamTex();
        // 3 colors for circles in the transition
        var colArr = new Color[3];
        colArr[0] = new Vector4(154, 209, 4, 1) / 255;
        colArr[1] = new Vector4(200, 33, 155, 1) / 255;
        colArr[2] = Color.white;                                                // will just be replaced with this camera texture
        mat.SetColorArray(materialColArrId, colArr);

        // delay time for each circle's appearance
        DelayArr[0] = 0.2f;
        DelayArr[1] = 0.4f;
        DelayArr[2] = 0.6f;
        mat.SetFloatArray(materialDelayArrId, DelayArr);

        // center of the circle's (added to (0.5, 0.5))
        var CenterArr = new Vector4[3];
        CenterArr[0] = new Vector2(0.5f, 0.3f);
        CenterArr[1] = new Vector2(-0.35f, -0.2f);
        CenterArr[2] = new Vector2(0.4f, -0.2f);
        mat.SetVectorArray(materialCenterArrId, CenterArr);

        ResetTransitionMat();                                                    // resets shader timer (shader trigger)
        mat.SetFloat(materialDurationId, duration);                              // duration of each circle
    }
    private void SetCamTex()
    {
        var tex = new RenderTexture(Screen.width, Screen.height, 8);            // creates texture 
        Debug.Assert(tex.Create(), "Failed");
        if (cam.targetTexture != null) { cam.targetTexture.Release(); }
        cam.targetTexture = tex;                                                // camera's screen output to texture
        mat.SetTexture(materialBlendTexId, tex);                                // texture is set as the blend texture in the settings transition material
    }
}
