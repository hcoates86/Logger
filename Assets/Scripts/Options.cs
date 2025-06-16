using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;

public class Options : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle archivedToggle;
    // public static bool mainScreenNormalView = true;
    public static bool playSounds = true;
    public static bool showArchivedBirthdays = true;

    private int currentInt;

    void Start()
    {
        // if keys don't exist sets them to true
        if (!PlayerPrefs.HasKey("playSounds"))
            PlayerPrefs.SetInt("playSounds", 1);
        if (!PlayerPrefs.HasKey("showArchivedBirthdays"))
            PlayerPrefs.SetInt("showArchivedBirthdays", 1);           

        LoadOptions();
    }

    // Saves on toggle
    public void ChangeSounds(bool isOn)
    {
        Debug.Log("ChangeSounds Bool value: " + isOn);
        if (isOn)
        {
            playSounds = true;
            PlayerPrefs.SetInt("playSounds", 1);
        }
        else
        {
            playSounds = true;
            PlayerPrefs.SetInt("playSounds", 0);    
        }
    }

    public void ChangeArchived(bool isOn)
    {
        Debug.Log("ChangeArchived Bool value: " + isOn);
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

    public void CheckPlayerPrefsValues()
    {
        Debug.Log("playSounds " + PlayerPrefs.GetInt("playSounds"));
        Debug.Log("showArchivedBirthdays " + PlayerPrefs.GetInt("showArchivedBirthdays"));
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
