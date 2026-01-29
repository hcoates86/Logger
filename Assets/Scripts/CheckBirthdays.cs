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
        bool isBirthday = false;
        // checks for a leap day birthdate. Considers both the 28th and 1st as a birthday for the 29th.
        if (birthDate.Month == 2 && birthDate.Day == 29)
        {
            if ((today.Month == 2 && today.Day == 28) ||
            (today.Month == 3 && today.Day == 1))
            isBirthday = true;
        }
        if (birthDate.Month == today.Month && birthDate.Day == today.Day)
            isBirthday = true;

        if (isBirthday)
        {
            return true;
        }
        else return false;
    }
}
