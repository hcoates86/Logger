using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    public AudioClip buttonClick;
    public AudioClip deleteEvent;
    public AudioClip deleteProfile;
    public AudioClip cancel;
    public AudioClip confirm;
    public AudioClip click;
    public AudioClip switchProfile;
    public AudioClip sort;
    public AudioClip errorClip;
    public AudioClip fullView;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject); // Destroy other instance
        }
    }

    public void PlayClip(AudioClip audioClip)
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(audioClip);
    }


    public void PlayCancel()
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(cancel);
    }
    
    public void PlayConfirm()
    {
        if (Options.playSounds)
            audioSource.PlayOneShot(confirm);
    }
    
    public void PlayButtonClick()
    {
        if (Options.playSounds)
            audioSource.PlayOneShot(buttonClick);
    }

    public void PlayClick()
    {
        if (Options.playSounds)
            audioSource.PlayOneShot(click);
    }

    public void PlayError()
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(errorClip);
    }

    public void PlaySort()
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(sort);
    }

    public void PlaySwitch()
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(switchProfile);
    }

    public void PlayFullView()
    {
        if (Options.playSounds)
        audioSource.PlayOneShot(fullView);
    }


}
