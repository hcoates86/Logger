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
    public Sprite profileImage;
    // // for vaccines, flea meds, etc
    // public List<string> listItems;
    // three favorite events
    public int[] favoriteEvents = new int[3];

    // keeps a count of the total events for event id and sorting
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

    void DeleteEvent(int eventId)
    {
        //fade + delete the gameobject 
        //delete the notification if notifications on that event were on


        // deletes the event file
        string path = $"{Application.persistentDataPath}/profiles/{id}/{eventId}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"File at {path} has been deleted.");
        }
        else
        {
            Debug.Log($"File at {path} does not exist.");
        }

    }

    void AddEvent()
    {

    }

    public void SaveEvent()
    {

    }


    [System.Serializable]
    public class ProfileData
    {
        public int id;
        public string named;
        public string birthDate;
        public string profileImagePath;
        public int favoriteEvent1;
        public int favoriteEvent2;
        public int favoriteEvent3;
        public int totalEventsAdded;
    }


    [System.Serializable]
    public class EventData
    {
        // corresponds to the profile id
        public int profileId;
        // to be loaded in order
        public int eventId;
        public string title;
        public string notes;
        //for when something was given, and when it's due next
        // should be able to edit these alone. Press edit button then click? If edit button has been pressed, then allow edit
        public string startDate;
        public string dueDate;
        public bool hasStartDate; 
        public bool hasDueDate;
    }

    public void SaveEventData(string title, string notes = "", string startDate = "", string dueDate = "", 
    bool hasStartDate = false, bool hasDueDate = false)
    {
        EventData data = new EventData();
        // assigns the profile's id
        data.profileId = id;

        int number = totalEventsAdded + 1;
        data.eventId = number;

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
        string itemPath = Path.Combine(path, $"{number}.json");
        File.WriteAllText(itemPath, json);
    }

    void OverwriteEventData()
    {
        // save to same id
    }

    void SaveProfile()
    {
        if (id == 0)
        {
            // assigns new id
            id = AppManager.Instance.allProfiles.Count + 1;

        }
        ProfileData data = new ProfileData();
        data.id = id;
        data.named = named;
        data.birthDate = birthDate.ToString("MM/dd/yyyy");
        // data.profileImagePath 
        data.favoriteEvent1 = favoriteEvents[0];
        data.favoriteEvent2 = favoriteEvents[1];
        data.favoriteEvent3 = favoriteEvents[2];
        data.totalEventsAdded = totalEventsAdded;

        string json = JsonUtility.ToJson(data);
        string path = $"{Application.persistentDataPath}/profiles";

        if (!Directory.Exists(path))
        {
            // Create the directory
            Directory.CreateDirectory(path);
        }

        File.WriteAllText($"{Application.persistentDataPath}/profiles/profile{id}.json", json);


    }

}
