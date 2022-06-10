using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider sensitivitySlider;
    private AudioManager audioManager;
    private SManager manager;

    private Camera mainCam;
    UniversalAdditionalCameraData mainCamData;                                          // used to switch renderer feature (change to index 1 for settings transition renderer)
    public SettingsCameraManager settingsCamM;

    [SerializeField]
    private Material diamondBGMat;                                                      // diamond material used for settings bg

    [SerializeField]
    private GameObject mainCanvas;
    void Start() {
        mainCam = Camera.main;
        mainCamData = mainCam.transform.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        audioManager = AudioManager.instance;
        manager = SManager.instance;
    }
    private void StartAnimationShader() {
        mainCamData.SetRenderer(1);                                                     // activates transition by switching to index 1 in main camera's renderer (should be set to settings transition renderer)
        var duration = settingsCamM.ResetTransitionMat();
        StartCoroutine(WaitForAnimation(duration));
    }

    // when main canvas is offscreen, it is turned off so settings can be interactable
    IEnumerator WaitForAnimation(float duration) {
        yield return new WaitForSeconds(duration / 2);
        mainCanvas.SetActive(false);
    }
    public void OpenMenu() {
        StartAnimationShader();
        manager.SwitchInteractable();
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivityOffset", 0);         // looks for player's saved settings
        audioManager.Play("SettingsClick");
    }

    public void CloseMenu() {
        audioManager.Play("SettingsClick");
        manager.SwitchInteractable();
        mainCanvas.SetActive(true);                                                     // turns on main canvas
        mainCamData.SetRenderer(0);
    }
}
