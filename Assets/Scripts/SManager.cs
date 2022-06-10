using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SManager : MonoBehaviour
{
    public static SManager instance;

    protected AudioManager audioManager;

    public bool interactable;

    static readonly int materialColorId = Shader.PropertyToID("_Color");
    //static readonly int materialTimeInitId = Shader.PropertyToID("_TimeInit");
    static readonly int materialCtrlId = Shader.PropertyToID("_Ctrl");
    static readonly int materialStepCtrlId = Shader.PropertyToID("_StepCtrl");

    

    protected virtual void Awake() {
        interactable = true;
        instance = this;
        audioManager = AudioManager.instance;
    }
    public void SwitchInteractable() { interactable = !interactable; }

    protected virtual void Update()
    {
    }
    public virtual void OnTransitionIn()
    {
        interactable = false;
    }
    public virtual void MatOnTransitionIn(Image bgTransitionIn, Material transitionMat = null, float _initial = 0f, float _target = 1f, float _duration = 0.75f)
    {
        /**/
        bgTransitionIn.gameObject.SetActive(true);
        bgTransitionIn.color = SceneCtrl.Instance.transitionColor;
        bgTransitionIn.material = transitionMat;
        /**/
        if (bgTransitionIn.material != null)
        {
            bgTransitionIn.material.SetColor(materialColorId, bgTransitionIn.color);
            // resets material
            bgTransitionIn.material.SetInt(materialCtrlId, 1);
            bgTransitionIn.material.SetFloat(materialStepCtrlId, 0);
            LeanTween.value(bgTransitionIn.gameObject, _initial, _target, _duration)
                .setEase(LeanTweenType.easeInQuad)
                .setOnUpdate((float val) => {
                    bgTransitionIn.material.SetFloat(materialStepCtrlId, val);
                })
                .setOnComplete(() => {
                    bgTransitionIn.material.SetFloat(materialStepCtrlId, 0);
                    bgTransitionIn.material = null;
                    bgTransitionIn.gameObject.SetActive(false);
                    interactable = true;
                });
        }
        /**/


        /**
        LeanTween.value(bgTransitionIn.gameObject, bgTransitionIn.color.a, 0, 0.9f)
            .setEase(LeanTweenType.easeInBounce)
            .setOnUpdate((float val) => {
                
                //bgTransitionIn.color = Utilities.SetColor(bgTransitionIn.color, val);
            })
            .setOnComplete(() => {
                Debug.Log("done");
                bgTransitionIn.gameObject.SetActive(false);
            });
        /**/
    }
    public void CEOnTransitionOut(Image bgTransitionOut)
    {
        interactable = false;
        bgTransitionOut.gameObject.SetActive(true);
        bgTransitionOut.color = SceneCtrl.Instance.transitionColor;
        bgTransitionOut.material = Resources.Load<Material>("Shaders/LtRSwipeTransitionMat");
        /**/
        if (bgTransitionOut.material != null)
        {
            bgTransitionOut.material.SetColor(materialColorId, bgTransitionOut.color);
            // resets material
            bgTransitionOut.material.SetInt(materialCtrlId, 1);
            bgTransitionOut.material.SetFloat(materialStepCtrlId, 0);
            LeanTween.value(bgTransitionOut.gameObject, 0f, 1f, 0.4f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnUpdate((float val) => {
                    bgTransitionOut.material.SetFloat(materialStepCtrlId, val);
                })
                .setOnComplete(() => {
                    bgTransitionOut.material.SetFloat(materialStepCtrlId, 1f);
                    var mat = bgTransitionOut.material;
                    bgTransitionOut.material = null;
                    mat.SetFloat(materialStepCtrlId, 0f);

                    var sceneManager = SceneCtrl.Instance;
                    sceneManager.LoadScene("Main Menu");
                });
        }
        /**/
    }
}
