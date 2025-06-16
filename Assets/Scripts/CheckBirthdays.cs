using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckBirthdays : MonoBehaviour
{
    private string birthdayNames = string.Empty;
    [SerializeField] private TMP_Text birthdayText;
    [SerializeField] private CanvasGroupToggle birthdayContainerToggle;

    private DateTime today;
    // Start is called before the first frame update
    void Start()
    {
        today = DateTime.Today;

        CheckAllBirthdays(Options.showArchivedBirthdays);
        if (birthdayNames.Length > 0)
        {
            birthdayText.text += birthdayNames + "!";
            birthdayContainerToggle.ShowElement();
        }
    }

    void CheckAllBirthdays(bool includeArchived)
    {
        foreach (Profile profile in AppManager.Instance.allProfiles)
        {
            if (!includeArchived)
            {
                // If option to show archived birthdays is off, skips checking archived profiles
                if (profile.isArchived) continue;
            }
            if (CheckIfBirthday(profile.birthDate))
            {
                // If string already contains names add an "and" and space
                if (birthdayNames.Length > 0)
                {
                    birthdayNames += $" and {profile.named}";
                }
                else
                    birthdayNames += profile.named;
            }

        }

    }


    bool CheckIfBirthday(DateTime birthDate)
    {
        bool isBirthday = birthDate.Month == today.Month && birthDate.Day == today.Day;

        if (isBirthday)
        {
            return true;
        }
        else return false;

    }
}
