using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Image fillBlink;
    private Color fillBlinkColor;

    public float smoothing;
    void Start()
    {
        fill.color = gradient.Evaluate(1f);
        fillBlinkColor = fillBlink.color;
        fillBlink.color = Color.clear;
        smoothing = 5f;
    }
    public void FixedUpdate()
    {
        if (slider.value < 0.1f) { slider.value = 0; }
        //adjusts slider if health is above 0
        if (slider.value != playerMovement.health & playerMovement.health >= 0)
        {
            fill.color = gradient.Evaluate((float)playerMovement.health / (float)playerMovement.maxHealth);
            slider.value = Mathf.Lerp(slider.value, playerMovement.health, smoothing * Time.deltaTime);
        }
        //bar blinks when health is at 0
        else if (slider.value == 0)
        {
            fillBlink.color = Color.Lerp(Color.clear, fillBlinkColor, Mathf.Sin(Time.time * 4));
        }
    }
}
