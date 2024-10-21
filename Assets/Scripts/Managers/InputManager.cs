using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private UIManager uiManager;
    private void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.SwitchPauseUI();
        }*/
    }
}
