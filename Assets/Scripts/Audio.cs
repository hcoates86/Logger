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
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayCancel()
    {
        audioSource.PlayOneShot(cancel);

    }
    public void PlayConfirm()
    {
        audioSource.PlayOneShot(confirm);

    }
    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(buttonClick);
    }
    public void PlayClick()
    {
        audioSource.PlayOneShot(click);
    }
    public void PlayError()
    {
        audioSource.PlayOneShot(errorClip);
    }
    public void PlaySort()
    {
        audioSource.PlayOneShot(sort);
    }
    public void PlaySwitch()
    {
        audioSource.PlayOneShot(switchProfile);
    }
    public void PlayFullView()
    {
        audioSource.PlayOneShot(fullView);
    }

}
