using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public Image pauseScreenPrefab;
    public Image pauseScreen;

    public Canvas canvas;
    public GameObject disabledUI;
    public List<Image> activeUI;

    private void Start()
    {
        pauseScreen = CreatePauseScreen(pauseScreenPrefab);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!activeUI.Contains(pauseScreen))
            {
                Time.timeScale = 0;
                EnableUI(pauseScreen);
            }
            else
            {
                Time.timeScale = 1;
                DisableUI(pauseScreen);
            }
        }
    }

    public Image CreatePauseScreen(Image prefab)
    {
        Image currentUI = Instantiate(prefab);
        currentUI.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Button[] buttons = currentUI.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => ResumeGame(currentUI));
        buttons[1].onClick.AddListener(() => QuitToMainMenu(currentUI));
        DisableUI(currentUI);
        return currentUI;
    }
    public void QuitToMainMenu(Image currentUI)
    {
        SceneManager.LoadScene("Menu");
        DisableUI(currentUI);
    }
    public void ResumeGame(Image currentUI)
    {
        DisableUI(currentUI);
    }
    public void DisableUI(Image currentUI)
    {
        activeUI.Remove(currentUI);
        currentUI.transform.SetParent(disabledUI.transform);
        if (activeUI.Count == 0)
        {
            Time.timeScale = 1;
        }
    }
    public void EnableUI(Image currentUI)
    {
        activeUI.Add(currentUI);
        currentUI.transform.SetParent(canvas.transform);
    }
}
