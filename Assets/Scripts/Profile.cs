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

    // keeps a count of the total events for event id and sorting
    public int totalEventsAdded;
    // public Transform eventContainer;
    public List<EventItem> allEvents = new List<EventItem>();

    public SortBy eventsSortedBy;
    // order profile should be displayed on the main page. Repeat numbers are allowed but will display in random order
    public int customSortNum;

    public Image profileBackground;

    void Start()
    {
        LoadEvents();
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
        if (AppManager.Instance.currentProfile != this)
            AppManager.Instance.SwitchProfile(this);
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
        data.totalEventsAdded = totalEventsAdded;
        data.eventsSortedBy = eventsSortedBy;
        data.customSortNum = customSortNum == 0 ? 500 : customSortNum;

        string json = JsonUtility.ToJson(data);
        string path = $"{Application.persistentDataPath}/profiles";

        if (!Directory.Exists(path))
        {
            // Create the directory
            Directory.CreateDirectory(path);
        }

        File.WriteAllText($"{Application.persistentDataPath}/profiles/profile{id}.json", json);
    }
    

    public void SortEvents(SortBy sortCriteria, bool scriptOnly = false)
    {

    // bool checks if sorting should be forced by script and not clicked. 
    // Sets previous to none before sorting again to avoid logic that checks eventsSortedBy
        if (scriptOnly)
        {
            eventsSortedBy = SortBy.None;
        }


        if (sortCriteria == SortBy.None)
        {
            eventsSortedBy = SortBy.None;

            allEvents.Sort((x, y) => y.isFavorite.CompareTo(x.isFavorite));
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
        }
        else if (sortCriteria == SortBy.DueDate)
        {
            eventsSortedBy = SortBy.DueDate;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.hasDueDate ? DateTime.Parse(e.dueDate.text) : DateTime.MaxValue)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
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
        }
        else if (sortCriteria == SortBy.GivenDate)
        {
            eventsSortedBy = SortBy.GivenDate;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenByDescending(e => e.hasStartDate ? DateTime.Parse(e.startDate.text) : DateTime.MinValue)
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
        else if (sortCriteria == SortBy.Created)
        {
            eventsSortedBy = SortBy.Created;

            var sortedEvents = allEvents
            .OrderByDescending(e => e.isFavorite)
            .ThenBy(e => e.eventId)
            .ToList();

            allEvents = new List<EventItem>(sortedEvents);
        }

        // doesn't bother rearranging everything if there are fewer than 2 events
        if (allEvents.Count < 2) return;

        // places the gameobjects in order in the hierarchy
        for (int i = 0; i < allEvents.Count; i++)
        {
            allEvents[i].transform.SetSiblingIndex(i);
        }

        LayoutRebuilder.MarkLayoutForRebuild(AppManager.Instance.fullProfile.eventContainer);
    }

    public void SortByCurrentCriteria()
    {
        SortEvents(eventsSortedBy, true);
        // places the gameobjects in order in the hierarchy
        // for (int i = 0; i < allEvents.Count; i++)
        // {
        //     allEvents[i].transform.SetSiblingIndex(i);
        // }

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

                    // parses the ids
                    int profileId = int.Parse(data[0]);
                    int eventId = int.Parse(data[1]);

                    // creates the event from the prefab but doesn't save it since it just loaded it
                    CreateEventGameObject(profileId, eventId, data[2], data[3], data[4], data[5], bool.Parse(data[6]), bool.Parse(data[7]), bool.Parse(data[8]));
                }
            }
        }

        SortByCurrentCriteria();
    }


    void CreateEventGameObject(int profileId, int eventId, string title, string notes, string startDate, 
    string dueDate, bool hasStartDate, bool hasDueDate, bool isFavorite)
    {
        // instantiates a new event and adds all info
        GameObject newEvent = Instantiate(AppManager.Instance.eventPrefab);
        EventItem eventItem = newEvent.GetComponent<EventItem>();
        eventItem.profileId = profileId;
        eventItem.eventId = eventId;
        eventItem.title.text = title;
        eventItem.notes.text = notes;
        eventItem.startDate.text = startDate;
        eventItem.dueDate.text = dueDate;
        eventItem.hasStartDate = hasStartDate;
        eventItem.hasDueDate = hasDueDate;
        eventItem.isFavorite = isFavorite;

        eventItem.DisplayOptional(true);
        eventItem.ChangeFavorite();

        // places event on the full profile
        eventItem.transform.SetParent(AppManager.Instance.fullProfile.eventContainer, false);
        // adds to the list and then sorts it
        allEvents.Add(eventItem);
        SortByCurrentCriteria();

        // turns off the event so it won't automatically show on all profiles
        eventItem.gameObject.SetActive(false);

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
    // for events and profile
    None, DueDate, GivenDate, ReverseGivenDate, ReverseDueDate, Created, ReverseCreated,
    // for profile sorting on appmanager
    Alphabetical, ReverseAlphabetical, Age, ReverseAge, Custom, ReverseCustom
}

public enum Save
{
    ID, Name, BDay, TotalEvents, SortedBy
}

    [System.Serializable]
    public class ProfileData
    {
        public int id;
        public string named;
        public string birthDate;
        public int totalEventsAdded;
        public SortBy eventsSortedBy;
        public int customSortNum;
    }