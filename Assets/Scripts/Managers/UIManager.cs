using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameStateManager gameStateManager;

    private GameObject canvas;
    private GameObject pauseUI;

    private GameObject upgradeUI;
    [SerializeField]
    private GameObject upgradeButtonPrefab;

    private GameObject abilityUI;
    private GameObject dashAbilityUI;
    private bool dashAbilityEnabled = true;
    private GameObject reflectAbilityUI;
    private bool reflectAbilityEnabled = true;

    private GameObject waveUI;
    private Text waveCountdownText;

    private EventSystem eventSystem;
    private PlayerInput playerInput;

    private GameObject firstSelected = null;

    private string currentControlScheme;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        canvas = GameObject.Find("Canvas");

        upgradeUI = GameObject.Find("UpgradeUI");
        upgradeUI.SetActive(false);

        pauseUI = GameObject.Find("PauseUI");
        pauseUI.SetActive(false);

        abilityUI = GameObject.Find("AbilityUI");
        dashAbilityUI = GameObject.Find("DashAbility");
        reflectAbilityUI = GameObject.Find("ReflectAbility");

        waveUI = GameObject.Find("WaveUI");
        waveCountdownText = waveUI.transform.Find("Countdown").GetComponent<Text>();
        waveUI.SetActive(false);

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePauseUI();
        }
    }

    private void Update()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;
            OnControlsChanged();
        }
    }

    public void EnablePauseUI()
    {
        if (upgradeUI.activeSelf) return;

        gameStateManager.ToPaused();
        pauseUI.SetActive(true);
        SetFirstSelectedIfGamepad(pauseUI.transform.Find("ResumeButton").gameObject);
    }

    public void DisablePauseUI()
    {
        gameStateManager.ToRunning();
        pauseUI.SetActive(false);
        RemoveFirstSelected();
    }

    public void TogglePauseUI()
    {
        if (pauseUI.activeSelf)
        {
            DisablePauseUI();
        }
        else
        {
            EnablePauseUI();
        }
    }

    public void EnableUpgradeUI()
    {
        Class playerClass = player.playerClass;
        if (playerClass.upgrades.Count == 0) return;

        gameStateManager.ToPaused();

        Transform buttons = upgradeUI.transform.Find("Buttons");

        // First make sure that the amount of buttons and upgrades are the same
        if (buttons.childCount > playerClass.upgrades.Count)
        {
            for (int i = playerClass.upgrades.Count; i < buttons.childCount; i++)
            {
                GameObject child = buttons.GetChild(i).gameObject;
                child.transform.SetParent(null);
                if (child != null) Destroy(child);
            }
        }
        else
        {
            for (int i = buttons.childCount; i < playerClass.upgrades.Count; i++)
            {
                Instantiate(upgradeButtonPrefab, buttons);
            }
        }

        // Link each button to upgrading the player to that class
        for (int i = 0; i < buttons.childCount; i++)
        {
            Transform buttonTransform = buttons.GetChild(i);
            Class currentPlayerClass = playerClass.upgrades[i];

            Text text = buttonTransform.GetComponentInChildren<Text>();
            text.text = currentPlayerClass.className;

            Image image = buttonTransform.Find("Sprite").GetComponent<Image>();
            if (player.blue) image.sprite = currentPlayerClass.blueSprite;
            else image.sprite = currentPlayerClass.redSprite;

            Button button = buttonTransform.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { 
                player.ApplyClass(currentPlayerClass, player.blue);
                DisableUpgradeUI();
            });
        }

        upgradeUI.SetActive(true);
        SetFirstSelectedIfGamepad(buttons.GetChild(0).gameObject);
    }

    public void DisableUpgradeUI()
    {
        gameStateManager.ToRunning();
        upgradeUI.SetActive(false);
        RemoveFirstSelected();
    }

    public void DisableDashAbility()
    {
        if (!dashAbilityEnabled) return;
        dashAbilityEnabled = false;

        DashAbility dashAbility = player.GetComponent<DashAbility>();
        int cooldown = dashAbility.dashCooldown;

        Image image = dashAbilityUI.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.1f);

        Text text = dashAbilityUI.GetComponentInChildren<Text>();
        text.text = cooldown.ToString();

        StartCoroutine(ReduceCountEverySecond(text));
    }

    public void EnableDashAbility()
    {
        dashAbilityEnabled = true;

        Image image = dashAbilityUI.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);

        Text text = dashAbilityUI.GetComponentInChildren<Text>();
        text.text = "";
    }

    public void DisableReflectAbility()
    {
        if (!reflectAbilityEnabled) return;
        reflectAbilityEnabled = false;

        ReflectShieldAbility reflectAbility = player.GetComponent<ReflectShieldAbility>();
        int cooldown = reflectAbility.reflectShieldCooldown;

        Image image = reflectAbilityUI.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.1f);

        Text text = reflectAbilityUI.GetComponentInChildren<Text>();
        text.text = cooldown.ToString();

        StartCoroutine(ReduceCountEverySecond(text));
    }

    public void EnableReflectAbility()
    {
        reflectAbilityEnabled = true;

        Image image = reflectAbilityUI.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);

        Text text = reflectAbilityUI.GetComponentInChildren<Text>();
        text.text = "";
    }

    public void EnableWaveUI(int countdown)
    {
        if (waveUI.activeSelf) return;

        waveUI.SetActive(true);
        waveCountdownText.text = countdown.ToString();

        StartCoroutine(ReduceCountEverySecond(waveCountdownText));
    }

    public void DisableWaveUI()
    {
        waveCountdownText.text = "";
        waveUI.SetActive(false);
    }

    private IEnumerator ReduceCountEverySecond(Text text)
    {
        yield return new WaitForSeconds(1);
        if (text.text != "")
        {
            int cooldown = int.Parse(text.text);
            if (cooldown > 0)
            {
                text.text = (cooldown - 1).ToString();
                StartCoroutine(ReduceCountEverySecond(text));
            }
        }
    }

    private void SetFirstSelectedIfGamepad(GameObject obj)
    {
        firstSelected = obj;

        // Only select first if player is using a gamepad
        if (Gamepad.current != null && Gamepad.current.enabled && playerInput.currentControlScheme == "Gamepad") eventSystem.SetSelectedGameObject(obj);
    }

    private void RemoveFirstSelected()
    {
        firstSelected = null;
        eventSystem.SetSelectedGameObject(null);
    }

    public void OnControlsChanged()
    {
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
