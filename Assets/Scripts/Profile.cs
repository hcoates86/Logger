using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Profile : MonoBehaviour
{
    public string named;
    public DateTime birthDate;
    public int Age => ComputeAge();
    public List<string> vaccines;
    public string notes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    int ComputeAge()
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age)) age--;

        return age;
    }

}
