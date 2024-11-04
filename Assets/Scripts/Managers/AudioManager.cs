using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    private AudioSource effectsAudioSource;
    private AudioSource musicAudioSource;

    public AudioClip UINavigationSound;
    public AudioClip UIClickSound;

    public AudioClip ShootSound;
    public AudioClip DieSound;

    public AudioClip mainMenuSound;
    public AudioClip gameSound;

    [Range(0f, 1f)]
    public float effectsVolume = 1;

    [Range(0f, 1f)]
    public float musicVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        effectsAudioSource = this.transform.Find("EffectsAudioSource").GetComponent<AudioSource>();
        musicAudioSource = this.transform.Find("MusicAudioSource").GetComponent<AudioSource>();
    }

    public void PlayHoverSound(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            effectsAudioSource.PlayOneShot(UINavigationSound, 0.02f * effectsVolume);
        }
    }

    public void PlayNavigateSound(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            effectsAudioSource.PlayOneShot(UINavigationSound, 0.02f * effectsVolume);
        }
    }

    public void PlayClickSound()
    {
        effectsAudioSource.PlayOneShot(UIClickSound, 0.02f * effectsVolume);
    }

    public void PlayDieSound()
    {
        effectsAudioSource.PlayOneShot(DieSound, 0.02f * effectsVolume);
    }

    public void PlayShootSound()
    {
        effectsAudioSource.PlayOneShot(ShootSound, 0.03f * effectsVolume);
    }
}
