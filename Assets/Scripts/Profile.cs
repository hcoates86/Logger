using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Faisalman.AgeCalc;

public class Profile : MonoBehaviour
{
    public string named;
    // must be in day/month/year format
    public DateTime birthDate;
    public string Age => GetAge();
    // for vaccines, flea meds, etc
    public List<string> listItems;
    public string notes;
    // allows editing of profile items
    public bool edit = false;

    string GetAge()
    {
        DateTime currentDate = DateTime.Now;
        Age age = new Age(birthDate, currentDate);

        if (age.Years < 1)
        {
            if (age.Months < 1)
            {
                return $"{age.Days / 7} Weeks, {age.Days} Days";

            }

            return $"{age.Months} Months, {age.Days} Days";
        }

        return $"{age.Years} Years";
    }

}
