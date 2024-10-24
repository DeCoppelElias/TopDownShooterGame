using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private enum State {RUNNING, SLOWINGDOWN, SLOWMO, SPEEDUP, PAUSED}

    [SerializeField]
    private State state;

    private float finalTimeScale = 1;
    private float slowDownStart = 0;
    private float slowDownDuration = 2;
    private float timeScaleDuration = 0.5f;
    private float timeScaleStart = 0;
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        this.volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        this.volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }

    private void Update()
    {
        if (this.state == State.SLOWINGDOWN)
        {
            // Calculating current time scale and applying
            float percentage = (Time.realtimeSinceStartup - timeScaleStart) / timeScaleDuration;
            float currentTimeScale = 1 - ((1 - finalTimeScale) * percentage);
            if (currentTimeScale < 0) currentTimeScale = 0;
            Time.timeScale = currentTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            // Applying color adjustment as time is slowed
            if (colorAdjustments != null) colorAdjustments.saturation.value = -100 * percentage;

            if (Time.realtimeSinceStartup - timeScaleStart > timeScaleDuration)
            {
                slowDownStart = Time.realtimeSinceStartup;
                state = State.SLOWMO;
            }
        }

        else if (state == State.SPEEDUP)
        {
            // Calculating current time scale and applying
            float percentage = (Time.realtimeSinceStartup - timeScaleStart) / timeScaleDuration;
            float currentTimeScale = finalTimeScale + ((1 - finalTimeScale) * percentage);
            if (currentTimeScale > 1) currentTimeScale = 1;
            Time.timeScale = currentTimeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;

            // Removing color adjustment as time is speeding up again
            if (colorAdjustments != null) colorAdjustments.saturation.value = -100 * (1 - percentage);

            if (Time.realtimeSinceStartup - timeScaleStart > timeScaleDuration)
            {
                state = State.RUNNING;
            }
        }

        else if (state == State.SLOWMO)
        {
            if (Time.realtimeSinceStartup - slowDownStart > slowDownDuration)
            {
                timeScaleStart = Time.realtimeSinceStartup;
                state = State.SPEEDUP;
            }
        }
    }
    public void ToPaused()
    {
        this.state = State.PAUSED;
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void SlowDownTime(float finalTimeScale, float timeScaleDuration, float slowDownDuration)
    {
        this.finalTimeScale = finalTimeScale;
        this.timeScaleDuration = timeScaleDuration;
        this.slowDownDuration = slowDownDuration;

        if (state == State.RUNNING || state == State.SPEEDUP)
        {
            state = State.SLOWINGDOWN;
            timeScaleStart = Time.realtimeSinceStartup;
        }
        else if (state == State.SLOWMO)
        {
            slowDownStart = Time.realtimeSinceStartup;
        }
        else if (state == State.SLOWINGDOWN)
        {
            timeScaleStart = Time.realtimeSinceStartup;
        }

    }

    public void ToRunning()
    {
        this.state = State.RUNNING;
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public bool IsPaused()
    {
        return this.state == State.PAUSED;
    }

    public void QuitToMainMenu()
    {
        ToRunning();

        // Clean up all enemies
        Transform enemiesParent = GameObject.Find("Enemies").transform;
        foreach (Transform child in enemiesParent)
        {
            Destroy(child.gameObject);
        }

        // Clean up all bullets
        Transform bulletsParent = GameObject.Find("Bullets").transform;
        foreach (Transform child in bulletsParent)
        {
            Destroy(child.gameObject);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
