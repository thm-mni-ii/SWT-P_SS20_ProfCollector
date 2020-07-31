using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class represents a blinking effect on an image.
/// The blinking is infinite until stopped.
/// </summary>
public class BlinkingUILoop : MonoBehaviour
{
    /// <summary>
    /// UI to toggle.
    /// </summary>
    [SerializeField] private Image uiToToggle;

    /// <summary>
    /// Blinks per second
    /// </summary>
    [SerializeField] private float blinkRate;

    /// <summary>
    /// Maximum alpha value in blink
    /// </summary>
    [SerializeField] private float maxAlpha;

    /// <summary>
    /// Stores the rest time of the current blink
    /// </summary>
    private float blinkTimer;

    /// <summary>
    /// Toggles if the blinking is active.
    /// </summary>
    private bool isActive;

    /// <summary>
    /// Toggles if the blinking is fading or getting brighter
    /// </summary>
    private bool isFading;

    /// <summary>
    /// Original alpha value
    /// </summary>
    private float startAlpha;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        startAlpha = uiToToggle.color.a;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Handles the blinking if active.
    /// </summary>
    private void Update()
    {
        if (isActive)
        {
            if (blinkTimer > 0f)
            {
                blinkTimer -= Time.deltaTime;

                Color fadedColor = uiToToggle.color;

                if (isFading)
                {
                    fadedColor.a = maxAlpha * (blinkTimer / blinkRate);
                }
                else
                {
                    fadedColor.a = maxAlpha * (1 - (blinkTimer / blinkRate));
                }

                uiToToggle.color = fadedColor;

                if (blinkTimer <= 0f)
                {
                    isFading = !isFading;
                    blinkTimer = blinkRate;
                }
            }
        }
    }

    /// <summary>
    /// Starts the blinking.
    /// </summary>
    public void StartBlink()
    {
        isFading = true;
        blinkTimer = blinkRate;
        isActive = true;
    }

    /// <summary>
    /// Stops the blinking.
    /// </summary>
    public void StopBlinking()
    {
        isActive = false;
        Color oldColor = uiToToggle.color;
        oldColor.a = startAlpha;
        uiToToggle.color = oldColor;
        blinkTimer = 0f;
    }


    #region Getter

    public bool getFading() => isFading;
    public float getblinkTimer() => blinkTimer;
    public bool getActiveStatus() => isActive;

    #endregion

    #region Setter

    public void setBlinkRate(float rate) => blinkRate = rate;
    public void setUIImage(Image img) => uiToToggle = img;

    #endregion

}