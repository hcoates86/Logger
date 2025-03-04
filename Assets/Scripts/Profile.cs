using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Faisalman.AgeCalc;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class Profile : MonoBehaviour
{
    public int id;
    public string named;
    // must be in day/month/year format
    public DateTime birthDate;
    public string Age => GetAge();
    public Sprite profileImage;
    public Sprite thumbnail;
    // // for vaccines, flea meds, etc
    // public List<string> listItems;
    // three favorite events
    // public int[] favoriteEvents = new int[3];

    // keeps a count of the total events for event id and sorting
    public int totalEventsAdded;
    // public Transform eventContainer;
    public List<EventItem> allEvents = new List<EventItem>();

    public SortBy eventsSortedBy;

    public Image profileBackground;

    void Start()
    {
        SortByCurrentCriteria();
    }

    string GetAge()
    {
        // if date is default value, treat as null
        if (birthDate == DateTime.MinValue)
            return "";

        // DateTime currentDate = DateTime.Now;

        // since the age calc can't handle future dates, just list as unborn until date
        if (birthDate > DateTime.Now)
        {
            return "Unborn";
            // TODO: "Due in x weeks/months"
        }

        Age age = new Age(birthDate, DateTime.Now);

        string pluralDays;
        string pluralWeeks;
        string pluralMonths;
        string pluralYears;
        if (age.Days == 1)
            pluralDays = "Day";
        else
            pluralDays = "Days";
        if (Mathf.Floor(age.Days / 7) == 1)
            pluralWeeks = "Week";
        else
            pluralWeeks = "Weeks";

        if (age.Months == 1)
            pluralMonths = "Month";
        else
            pluralMonths = "Months";
        if (age.Years == 1)
            pluralYears = "Year";
        else
            pluralYears = "Years";


        if (age.Years < 1)
        {
            if (age.Months < 1)
            {
                return $"{Mathf.Floor(age.Days / 7)} {pluralWeeks} {age.Days} {pluralDays}";

            }



            return $"{age.Months} {pluralMonths} {Mathf.Floor(age.Days / 7)} {pluralWeeks}";
        }

        return $"{age.Years} {pluralYears} {age.Months} {pluralMonths}";
    }

    public void OnClick()
    {
        AppManager.Instance.SwitchProfile(this);
    }

    // [System.Serializable]
    // public class ProfileData
    // {
    //     public int id;
    //     public string named;
    //     public string birthDate;
    //     // public string profileImagePath;
    //     public int favoriteEvent1;
    //     public int favoriteEvent2;
    //     public int favoriteEvent3;
    //     public int totalEventsAdded;
    //     public SortBy eventsSortedBy;
    // }

    // [System.Serializable]
    // public class EventData
    // {
    //     // corresponds to the profile id
    //     public int profileId;
    //     // to be loaded in order
    //     public int eventId;
    //     public string title;
    //     public string notes;
    //     //for when something was given, and when it's due next
    //     // should be able to edit these alone. Press edit button then click? If edit button has been pressed, then allow edit
    //     public string startDate;
    //     public string dueDate;
    //     public bool hasStartDate; 
    //     public bool hasDueDate;
    //     public bool isFavorite;
    // }

    public void SaveEventData(string title, bool isFavorite, string notes = "", string startDate = "", string dueDate = "", 
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
        data.isFavorite = isFavorite;


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

    public void SaveProfile()
    {
        if (id == 0)
        {
            // assigns new id
            id = AppManager.Instance.CreateNewId();

        }
        ProfileData data = new ProfileData();
        data.id = id;
        data.named = named;
        data.birthDate = birthDate.ToString("MM/dd/yyyy");
        // data.profileImagePath 
        // data.favoriteEvent1 = favoriteEvents[0];
        // data.favoriteEvent2 = favoriteEvents[1];
        // data.favoriteEvent3 = favoriteEvents[2];
        data.totalEventsAdded = totalEventsAdded;
        data.eventsSortedBy = eventsSortedBy;

        string json = JsonUtility.ToJson(data);
        string path = $"{Application.persistentDataPath}/profiles";

        if (!Directory.Exists(path))
        {
            // Create the directory
            Directory.CreateDirectory(path);
        }

        File.WriteAllText($"{Application.persistentDataPath}/profiles/profile{id}.json", json);
    }

    // bool checks if sorting should be forced by script and not clicked. 
    // ^Sets previous to none before sorting again to avoid click-based logic
    public void SortEvents(SortBy sortCriteria, bool scriptOnly = false)
    {
        if (scriptOnly)
        {
            eventsSortedBy = SortBy.None;
        }


        if (sortCriteria == SortBy.None)
        {
            eventsSortedBy = SortBy.None;

            allEvents.Sort((x, y) => y.isFavorite.CompareTo(x.isFavorite));
        }

        DateTime now = DateTime.Now;



        if (sortCriteria == SortBy.DueDate)
        {
            eventsSortedBy = SortBy.DueDate;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.hasDueDate ? DateTime.Parse(e.dueDate.text) : DateTime.MaxValue)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);

            // allEvents.Sort((x, y) =>
            // {
            //     // First, compare by isfavorite (true first)
            //     int favoriteComparison = y.isFavorite.CompareTo(x.isFavorite);
            //     if (favoriteComparison != 0)
            //         return favoriteComparison;

            //     // Then, compare by enddate closest to now
            //     DateTime endDateX = DateTime.Parse(x.dueDate.text);
            //     DateTime endDateY = DateTime.Parse(y.dueDate.text);
            //     double diffX = (endDateX - now).TotalSeconds;
            //     double diffY = (endDateY - now).TotalSeconds;

            //     return diffX.CompareTo(diffY);
            // });
        }
        // if events are already sorted by due date, sort by ascending duedate (for button click)
        // also set explicitely for script setup
        if ((sortCriteria == SortBy.DueDate && eventsSortedBy == SortBy.DueDate)
            || sortCriteria == SortBy.ReverseDueDate)
        {
            eventsSortedBy = SortBy.DueDate;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenByDescending(e => e.hasDueDate ? DateTime.Parse(e.dueDate.text) : DateTime.MinValue)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);

            // allEvents.Sort((x, y) =>
            // {
            //     // First, compare by isfavorite (true first)
            //     int favoriteComparison = y.isFavorite.CompareTo(x.isFavorite);
            //     if (favoriteComparison != 0)
            //         return favoriteComparison;

            //     // Then, compare by enddate closest to now
            //     DateTime endDateX = DateTime.Parse(x.dueDate.text);
            //     DateTime endDateY = DateTime.Parse(y.dueDate.text);
            //     double diffX = (endDateX - now).TotalSeconds;
            //     double diffY = (endDateY - now).TotalSeconds;

            //     return diffY.CompareTo(diffX);
            // });
        }
        // if events are already sorted by given date, sort by ascending givendate (for button click)
        // also set explicitely for script setup
        if ((sortCriteria == SortBy.GivenDate && eventsSortedBy == SortBy.GivenDate)
            || sortCriteria == SortBy.ReverseGivenDate)
        {
            eventsSortedBy = SortBy.ReverseGivenDate;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.hasStartDate ? DateTime.Parse(e.startDate.text) : DateTime.MaxValue)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
            // allEvents.Sort((x, y) =>
            // {
            //     // First, compare by isfavorite (true first)
            //     int favoriteComparison = y.isFavorite.CompareTo(x.isFavorite);
            //     if (favoriteComparison != 0)
            //         return favoriteComparison;

            //     // Then, compare by enddate closest to now
            //     DateTime startDateX = DateTime.Parse(x.startDate.text);
            //     DateTime startDateY = DateTime.Parse(y.startDate.text);
            //     return startDateX.CompareTo(startDateY);
            // });
        }
        if (sortCriteria == SortBy.GivenDate)
        {
            eventsSortedBy = SortBy.GivenDate;

            
            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenByDescending(e => e.hasStartDate ? DateTime.Parse(e.startDate.text) : DateTime.MinValue)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
            // allEvents.Sort((x, y) =>
            // {
            //     // First, compare by isfavorite (true first)
            //     int favoriteComparison = y.isFavorite.CompareTo(x.isFavorite);
            //     if (favoriteComparison != 0)
            //         return favoriteComparison;

            //     // Then, compare by enddate closest to now
            //     DateTime startDateX = DateTime.Parse(x.startDate.text);
            //     DateTime startDateY = DateTime.Parse(y.startDate.text);
            //     return startDateY.CompareTo(startDateX);
            // });
        }

        if (sortCriteria == SortBy.Created)
        {
            eventsSortedBy = SortBy.Created;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.eventId)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
        }
        if ((sortCriteria == SortBy.Created && eventsSortedBy == SortBy.Created)
            || sortCriteria == SortBy.ReverseCreated)
        {
            eventsSortedBy = SortBy.ReverseCreated;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.eventId)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
        }
    }

    public void SortByCurrentCriteria()
    {
        SortEvents(eventsSortedBy, true);
    }

    public void LoadEvents()
    {
        // all events are saved to this folder in format eventId.json
        string path = $"{Application.persistentDataPath}/{id}";
        string getJson = "*.json";

        AppManager.Instance.LoadEventData(path);

         if (Directory.Exists(path))
        {
            string[] filePaths = Directory.GetFiles(path, getJson);
            if (filePaths.Length > 0)
            {
                foreach (string filePath in filePaths)
                {
                    string[] data = AppManager.Instance.LoadEventData(filePath);
                    // DateTime dateValue;
                    // DateTime.TryParse(data[2], out dateValue);

                    // parses the id
                    int profileId = int.Parse(data[0]);


                    // creates the event from the prefab but doesn't save it since it just loaded it
                    CreateEventGameObject();
                }
            }
        }
    }


    void CreateEventGameObject()
    {
        GameObject newEvent = Instantiate(AppManager.Instance.eventPrefab);

    }

    // hides or shows all events
    public void ShowAllEvents(bool show)
    {
        foreach (EventItem item in allEvents)
        {
            //turns elements on or off
            // item.selfCGT.ShowElement(show);
            item.gameObject.SetActive(show);
        }
    }
}

public enum SortBy
{
    // none is effectively a cancel
    None, DueDate, GivenDate, ReverseGivenDate, ReverseDueDate, Created, ReverseCreated
}

    [System.Serializable]
    public class ProfileData
    {
        public int id;
        public string named;
        public string birthDate;
        // public string profileImagePath;
        // public int favoriteEvent1;
        // public int favoriteEvent2;
        // public int favoriteEvent3;
        public int totalEventsAdded;
        public SortBy eventsSortedBy;
    }

    // [System.Serializable]
    // public class EventData
    // {
    //     // corresponds to the profile id
    //     public int profileId;
    //     // to be loaded in order
    //     public int eventId;
    //     public string title;
    //     public string notes;
    //     //for when something was given, and when it's due next
    //     // should be able to edit these alone. Press edit button then click? If edit button has been pressed, then allow edit
    //     public string startDate;
    //     public string dueDate;
    //     public bool hasStartDate; 
    //     public bool hasDueDate;
    //     public bool isFavorite;
    // }
