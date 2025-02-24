using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Faisalman.AgeCalc;
using UnityEngine.UI;
using System.IO;

public class Profile : MonoBehaviour
{
    public int id;
    public string named;
    // must be in day/month/year format
    public DateTime birthDate;
    public string Age => GetAge();
    public Image profileImage;
    // for vaccines, flea meds, etc
    public List<string> listItems;
    // public string notes;
    // allows editing of profile items
    // public bool edit = false;

    private int totalEventsAdded;

    string GetAge()
    {
        DateTime currentDate = DateTime.Now;
        Age age = new Age(birthDate, currentDate);

        if (age.Years < 1)
        {
            if (age.Months < 1)
            {
                return $"{Mathf.Floor(age.Days / 7)} Weeks {age.Days} Days";

            }

            return $"{age.Months} Months {age.Days} Days";
        }

        return $"{age.Years} Years {age.Months} Months";
    }

    void DeleteEvent()
    {

    }

    void AddEvent()
    {

    }

    void SaveEvent()
    {

    }




    [System.Serializable]
    public class EventData
    {
        // corresponds to the profile id
        public int id;
        // event number, to be loaded in order
        public int number;
        public string title;
        public string notes;
        //for when something was given, and when it's due next
        // should be able to edit these alone. Press edit button then click? If edit button has been pressed, then allow edit
        public string startDate;
        public string dueDate;
        public bool hasStartDate; 
        public bool hasDueDate;
    }

    public void SaveItemData(string title, string notes = "", string startDate = "", string dueDate = "", 
    bool hasStartDate = false, bool hasDueDate = false)
    {
        EventData data = new EventData();
        // assigns the profile's id
        data.id = id;

        int number = totalEventsAdded + 1;
        data.number = number;

        // assigns passed-in data
        data.title = title;
        data.notes = notes;
        data.startDate = startDate;
        data.dueDate = dueDate;
        data.hasStartDate = hasStartDate;
        data.hasDueDate = hasDueDate;


        string json = JsonUtility.ToJson(data);
        string path = $"{Application.persistentDataPath}/profiles/{id}";

        if (!Directory.Exists(path))
        {
            // Create the directory
            Directory.CreateDirectory(path);
        }

        // File.WriteAllText($"{Application.persistentDataPath}/profiles/{id}/{}.json", json);
        string itemPath = Path.Combine(path, number.ToString());
        File.WriteAllText(itemPath, json);
    }

    void OverwriteItemData()
    {
        //load?
        //and save
    }

}
