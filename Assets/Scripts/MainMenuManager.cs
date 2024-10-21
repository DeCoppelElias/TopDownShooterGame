using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject firstSelected;
    public PlayerInput playerInput;

    private string currentControlScheme;

    private void Update()
    {
        if (playerInput.currentControlScheme != currentControlScheme)
        {
            ControlsChanged();
            currentControlScheme = playerInput.currentControlScheme;
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void PlayPVP()
    {
        SceneManager.LoadScene("PVP");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ControlsChanged()
    {
        if (!playerInput.enabled) return;

        if (Gamepad.current != null && Gamepad.current.enabled && playerInput.currentControlScheme == "Gamepad")
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
