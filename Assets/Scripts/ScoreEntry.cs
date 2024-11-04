using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    private bool initialised = false;
    private bool done = false;

    private string reason = "";
    private int score = 0;
    private int multiplier = 1;

    private Text reasonText;
    private Text scoreText;
    private Text multiplierText;

    [SerializeField] private float duration = 2;
    [SerializeField] private float durationAdd = 0.5f;
    private float initialiseTime = 0;

    private Action onTimeout;

    // Update is called once per frame
    void Update()
    {
        if (!initialised) return;

        if (Time.time - initialiseTime > duration)
        {
            onTimeout();
            Destroy(this.gameObject);
        }
    }

    public void Initialise(string reason, int initialScore, Action onTimeout)
    {
        reasonText = this.transform.Find("Reason").GetComponent<Text>();
        scoreText = this.transform.Find("Score").GetComponent<Text>();
        multiplierText = this.transform.Find("Multiplier").GetComponent<Text>();
        initialiseTime = Time.time;

        this.score = initialScore;
        this.reason = reason;

        reasonText.text = reason;
        scoreText.text = score.ToString();
        multiplierText.text = "x" + multiplier.ToString();

        this.onTimeout = onTimeout;

        initialised = true;
    }

    public int GetScore()
    {
        return score * multiplier;
    }

    public void AddScore(int additionalScore)
    {
        if (done || !initialised) return;

        this.score += additionalScore;
        this.multiplier += 1;

        scoreText.text = score.ToString();
        multiplierText.text = "x" + multiplier.ToString();

        // Give extra time
        this.initialiseTime += this.durationAdd;
    }
}
