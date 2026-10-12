using System;
using TMPro;
using UnityEngine;

public class ListItem : MonoBehaviour
{
    public TMP_Text named, date, age;

    public void Setup(Profile profile)
    {
        named.text = profile.named;
        age.text = profile.Age;

        string dateString;
        if (profile.birthDate != DateTime.MinValue)
            dateString = profile.birthDate.ToString("MM/dd/yyyy");
        else
            dateString = NewEventHandler.blankDate;

        if (profile.endDate != DateTime.MinValue)
        {
            dateString += " - " + profile.endDate.ToString("MM/dd/yyyy");
        }
        
        date.text = dateString;
    }
}
