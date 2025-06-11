using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBirthdays : MonoBehaviour
{
    private DateTime today;
    // Start is called before the first frame update
    void Start()
    {
        today = DateTime.Today;

        CheckAllBirthdays(AppManager.Instance.showArchivedBirthdays);
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
                if (AppManager.Instance.birthdayNames.Length > 0)
                {
                    AppManager.Instance.birthdayNames += $"and {profile.named}";
                }
                else
                    AppManager.Instance.birthdayNames += profile.named;
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
