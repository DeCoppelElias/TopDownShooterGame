using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private enum State {RUNNING, SLOWMO, PAUSED}

    [SerializeField]
    private State state;
    public void ToPaused()
    {
        this.state = State.PAUSED;
        Time.timeScale = 0;
    }

    public void ToSlowmo(float slow)
    {
        this.state = State.SLOWMO;
        Time.timeScale = slow;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void ToRunning()
    {
        this.state = State.RUNNING;
        Time.timeScale = 1;
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
