using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using System;

public class Options : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle archivedToggle;

    public static bool playSounds = false;
    public static bool showArchivedBirthdays = true;

    void Start()
    {
        // if keys don't exist sets them to default
        if (!PlayerPrefs.HasKey("playSounds"))
            PlayerPrefs.SetInt("playSounds", 0);
        if (!PlayerPrefs.HasKey("showArchivedBirthdays"))
            PlayerPrefs.SetInt("showArchivedBirthdays", 1);

        LoadOptions();
    }


    public void CheckSounds()
    {
        Debug.Log($"playSounds: {playSounds}");
    }

    // Saves on toggle
    public void ChangeSounds(bool isOn)
    {
        if (isOn)
        {
            playSounds = true;
            PlayerPrefs.SetInt("playSounds", 1);
        }
        else
        {
            playSounds = false;
            PlayerPrefs.SetInt("playSounds", 0);
        }
    }

    public void ChangeArchived(bool isOn)
    {
        if (isOn)
        {
            showArchivedBirthdays = true;
            PlayerPrefs.SetInt("showArchivedBirthdays", 1);
        }
        else
        {
            showArchivedBirthdays = false;
            PlayerPrefs.SetInt("showArchivedBirthdays", 0);

        }
    }

    void LoadOptions()
    {
        if (PlayerPrefs.GetInt("playSounds") == 0)
        {
            playSounds = false;
            soundToggle.isOn = false;
            // Makes the custom toggle visually update;
            soundToggle.GetComponent<CustomToggle>().UpdateState();

        }
        if (PlayerPrefs.GetInt("showArchivedBirthdays") == 0)
        {
            showArchivedBirthdays = false;
            archivedToggle.isOn = false;

            // Makes the custom toggle visually update;
            archivedToggle.GetComponent<CustomToggle>().UpdateState();
        }
    }
}