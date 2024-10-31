using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private AudioManager audioManager;
    private PlayerInput playerInput;
    public bool canPlaySound = false;

    private void OnEnable()
    {
        StartCoroutine(EnableNavigateSound());
    }
    private IEnumerator EnableNavigateSound()
    {
        canPlaySound = false;
        yield return new WaitForSecondsRealtime(0.1f);
        canPlaySound = true;
    }

    private void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
        playerInput = GetComponentInParent<UIInfo>().owner;
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (canPlaySound && audioManager != null) audioManager.PlayNavigateSound(playerInput);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canPlaySound && audioManager != null) audioManager.PlayHoverSound(playerInput);
    }

    private void PlayClickSound()
    {
        if (audioManager != null) audioManager.PlayClickSound();
    }
}
