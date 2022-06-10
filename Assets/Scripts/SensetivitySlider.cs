using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensetivitySlider : MonoBehaviour
{
    public void OnSliderValueChanged(float value)
    {
        PlayerPrefs.SetFloat("sensitivityOffset", value);
    }
}
