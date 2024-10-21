using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PVPManager : MonoBehaviour
{
    private Player player1;
    private PlayerInput playerInput1;
    private Player player2;
    private PlayerInput playerInput2;

    private Canvas canvas1;
    private GameObject pauseUI1;
    private GameObject upgradeUI1;

    private Canvas canvas2;
    private GameObject pauseUI2;
    private GameObject upgradeUI2;

    private Canvas sharedCanvas;
    private GameObject notificationUI;
    private Text notificationText;

    private GameStateManager gameStateManager;

    private enum PVPState { Initialising, DeviceLost, PVP, CountDown, Done}
    [SerializeField]
    private PVPState pvpState = PVPState.Initialising;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("Player 1").GetComponent<Player>();
        playerInput1 = player1.GetComponent<PlayerInput>();
        player2 = GameObject.Find("Player 2").GetComponent<Player>();
        playerInput2 = player2.GetComponent<PlayerInput>();

        canvas1 = GameObject.Find("Canvas 1").GetComponent<Canvas>();
        pauseUI1 = canvas1.transform.Find("PauseUI").gameObject;
        pauseUI1.SetActive(false);
        upgradeUI1 = canvas1.transform.Find("UpgradeUI").gameObject;
        upgradeUI1.SetActive(false);

        canvas2 = GameObject.Find("Canvas 2").GetComponent<Canvas>();
        pauseUI2 = canvas2.transform.Find("PauseUI").gameObject;
        pauseUI2.SetActive(false);
        upgradeUI2 = canvas2.transform.Find("UpgradeUI").gameObject;
        upgradeUI2.SetActive(false);

        sharedCanvas = GameObject.Find("SharedCanvas").GetComponent<Canvas>();
        notificationUI = sharedCanvas.transform.Find("NotificationUI").gameObject;
        notificationText = notificationUI.transform.Find("Title").GetComponent<Text>();

        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

        InputSystem.onDeviceChange += OnDeviceChange;

        Initialise();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added)
            {
                // Assign the newly connected gamepad to this player
                playerInput2.SwitchCurrentControlScheme(device);
            }
            else if (change == InputDeviceChange.Removed)
            {
                EnablePlayerMovement(false);
                pvpState = PVPState.DeviceLost;
                DisplayNotification("Gamepad disconnected! The game is paused!", -1);
            }
        }
    }

    private void Initialise()
    {
        pvpState = PVPState.Initialising;

        EnablePlayerMovement(false);

        DisplayNotification("Welcome to PVP, please plug in a controller in order to play together.", -1);
    }

    // Update is called once per frame
    void Update()
    {
        if ((pvpState == PVPState.Initialising || pvpState == PVPState.DeviceLost) && playerInput1.devices.Count > 0 && playerInput2.devices.Count > 0)
        {
            if (pvpState == PVPState.Initialising)
            {
                CountDownToStart(5, "The game will start in " + 5 + ".");
            }
            else if (pvpState == PVPState.DeviceLost)
            {
                CountDownToStart(4, "Gamepad reconnected! The game continues.");
            }
        }
    }

    public void PlayerDied(Player player)
    {
        if (player == player1)
        {
            EnablePlayerMovement(false);
            DisplayNotification("Player 2 won!", 4);
        }
        else
        {
            EnablePlayerMovement(false);
            DisplayNotification("Player 1 won!", 4);
        }

        pvpState = PVPState.Done;
        StartCoroutine(PerformAfterDelay(() => this.gameStateManager.QuitToMainMenu(), 5));
    }

    public void EnablePauseUI(Player player)
    {
        gameStateManager.ToPaused();

        (GameObject pauseUI, GameObject upgradeUI) = GetUI(player);
        pauseUI.SetActive(true);

        // Disable controls of other player
        PlayerInput otherPlayerInput = GetOtherInput(player);
        otherPlayerInput.enabled = false;

        // Set first selected if gamepad
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput.currentControlScheme == "Gamepad")
        {
            MultiplayerEventSystem multiplayerEventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
            multiplayerEventSystem.SetSelectedGameObject(pauseUI.transform.Find("ResumeButton").gameObject);
        }
    }

    public void DisablePauseUI(Player player)
    {
        (GameObject pauseUI, GameObject upgradeUI) = GetUI(player);
        pauseUI.SetActive(false);
        gameStateManager.ToRunning();

        // Enable controls of other player
        PlayerInput otherPlayerInput = GetOtherInput(player);
        otherPlayerInput.enabled = true;

        // Set first selected if gamepad
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput.currentControlScheme == "Gamepad")
        {
            MultiplayerEventSystem multiplayerEventSystem = playerInput.uiInputModule.GetComponent<MultiplayerEventSystem>();
            multiplayerEventSystem.SetSelectedGameObject(null);
        }
    }

    public void TogglePauseUI(Player player)
    {
        (GameObject pauseUI, GameObject upgradeUI) = GetUI(player);
        if (pauseUI.activeSelf)
        {
            DisablePauseUI(player);
        }
        else
        {
            EnablePauseUI(player);
        }
    }

    public PlayerInput GetOtherInput(Player player)
    {
        if (player == player1)
        {
            return player2.GetComponent<PlayerInput>();
        }
        else
        {
            return player1.GetComponent<PlayerInput>();
        }
    }

    private (GameObject pauseUI, GameObject upgradeUI) GetUI(Player player)
    {
        if (player == player1)
        {
            return (pauseUI1, upgradeUI1);
        }
        else
        {
            return (pauseUI2, upgradeUI2);
        }
    }

    public void DisplayNotification(string notification, float removeAfter)
    {
        notificationText.text = notification;

        if (removeAfter == -1) return;

        StartCoroutine(RemoveTextAfter(notificationText, removeAfter));
    }

    private void CountDownToStart(int seconds, string initialMessage)
    {
        pvpState = PVPState.CountDown;
        DisplayNotification(initialMessage, -1);
        StartCoroutine(CountDownToStartCoRoutine(seconds));
    }

    private IEnumerator CountDownToStartCoRoutine(int seconds)
    {
        yield return new WaitForSeconds(1);

        if (pvpState == PVPState.CountDown)
        {
            seconds -= 1;
            if (seconds> 0)
            {
                DisplayNotification("The game will start in " + (seconds) + ".", -1);
                StartCoroutine(CountDownToStartCoRoutine(seconds));
            }
            else
            {
                DisplayNotification("Start!", 2);
                pvpState = PVPState.PVP;
                EnablePlayerMovement(true);
            }
        }
    }

    private IEnumerator RemoveTextAfter(Text text, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        text.text = "";
    }

    private IEnumerator PerformAfterDelay(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        action();
    }

    private void EnablePlayerMovement(bool enable)
    {
        if (player1 != null)
        {
            if (!enable) player1.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            player1.GetComponent<PlayerController>().collectInput = enable;
        }
        if (player2 != null)
        {
            if (!enable) player2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            player2.GetComponent<PlayerController>().collectInput = enable;
        }
    }
}
