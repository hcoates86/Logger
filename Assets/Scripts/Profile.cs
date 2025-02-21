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
    public int Age => ComputeAge();
    public List<string> vaccines;
    public string notes;

    // Start is called before the first frame update
    void Start()
    {
        // birthDate = new DateTime(01,11,2023);
        // GetAge();
    }

    int ComputeAge()
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age)) age--;

        return age;
    }

    string GetAge()
    {
        DateTime birthDate = new DateTime(1990, 1, 1);
        DateTime currentDate = DateTime.Now;
        Age age = new Age(birthDate, currentDate);

        Debug.Log($"Age: {age}, {age.Years} years, {age.Months} months, {age.Days} days");
        return age.ToString();

    }

}
