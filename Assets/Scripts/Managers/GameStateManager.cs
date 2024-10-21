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

    public void ToSlowmo()
    {
        this.state = State.SLOWMO;
        Time.timeScale = 0.5f;
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
        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            GameObject enemy = enemiesParent.GetChild(i).gameObject;
            enemy.transform.SetParent(null);
            if (enemy != null) Destroy(enemy);
        }

        // Clean up all bullets
        Transform bulletsParent = GameObject.Find("Bullets").transform;
        for (int i = 0; i < bulletsParent.childCount; i++)
        {
            GameObject bullet = bulletsParent.GetChild(i).gameObject;
            bullet.transform.SetParent(null);
            if (bullet != null) Destroy(bullet);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
