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
    public string Age => isArchived ? archivedAge : AppManager.Instance.GetAge(birthDate);

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

    public bool isArchived;
    public string archivedAge;

    void Start()
    {
        LoadEvents();
    }

    // Archives the profile so it'll always display the same age
    public void ArchiveProfile()
    {
        archivedAge = Age;
        isArchived = true;
        SaveProfile();
        //reload short profile
        AppManager.Instance.shortProfile.Setup(this);
        // Reloads the full profile if it's up.
        if (AppManager.Instance.fullProfileToggle.IsElementVisible())
            AppManager.Instance.fullProfile.Setup(this);

    }

    public void RemoveFromArchive()
    {
        isArchived = false;
        SaveProfile();
        //reload short profile
        AppManager.Instance.shortProfile.Setup(this);
        // Reloads the full profile if it's up.
        if (AppManager.Instance.fullProfileToggle.IsElementVisible())
            AppManager.Instance.fullProfile.Setup(this);
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
        data.customSortNum = customSortNum;
        data.isArchived = isArchived;
        data.archivedAge = archivedAge;

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
    }

    public void LoadEvents()
    {
        // all events are saved to this folder in format eventId.json
        string path = $"{Application.persistentDataPath}/{id}";
        string getJson = "*.json";

        AppManager.Instance.LoadEventData(path);
        if (AppManager.Instance.debugMode)
        {
            Debug.Log("Attempting to load events from " + path);
            Debug.Log("Path exists? " + Directory.Exists(path));

        }

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

                    if (AppManager.Instance.debugMode)
                        Debug.Log($"Loading event with: {profileId}, {eventId}, {data[2]}, {data[3]}, {data[4]}, {data[5]}, {bool.Parse(data[6])}, {bool.Parse(data[7])}, {bool.Parse(data[8])}");

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
        GameObject newEvent;
        // instantiates a new event depending on if it should use short or expanded version, and adds all info
        if (notes != string.Empty)
            newEvent = Instantiate(AppManager.Instance.eventPrefab);
        else
            newEvent = Instantiate(AppManager.Instance.shortEventPrefab);

        EventItem eventItem = newEvent.GetComponent<EventItem>();
        eventItem.profileId = profileId;
        eventItem.eventId = eventId;
        eventItem.title.text = title;
        if (eventItem.notes != null)
            eventItem.notes.text = notes;
        eventItem.startDate.text = startDate;
        eventItem.dueDate.text = dueDate;
        eventItem.hasStartDate = hasStartDate;
        eventItem.hasDueDate = hasDueDate;
        eventItem.isFavorite = isFavorite;

        eventItem.DisplayOptional();
        eventItem.ChangeFavorite();

        // places event on the full profile
        eventItem.transform.SetParent(AppManager.Instance.fullProfile.eventContainer, false);
        // adds to the list and then sorts it
        allEvents.Add(eventItem);
        SortByCurrentCriteria();

        // turns off the event so it won't automatically show on all profiles
        eventItem.gameObject.SetActive(false);

    }

    //for copying over an event when short/expanded event needs to be created on note change
    // Takes an Event Item as the base item that will be replaced, and a bool depending on if it should use the short or expanded event
    public void CreateEventGameObject(EventItem baseEvent, bool useShort, string notes = "")
    {
        GameObject newEvent;
        // instantiates a new event depending on if it should use short or expanded version, and adds all info
        if (!useShort)
            newEvent = Instantiate(AppManager.Instance.eventPrefab);
        else
            newEvent = Instantiate(AppManager.Instance.shortEventPrefab);

        EventItem eventItem = newEvent.GetComponent<EventItem>();
        // this event should be this profile's, so use own id
        eventItem.profileId = id;
        eventItem.eventId = baseEvent.eventId;
        eventItem.title.text = baseEvent.title.text;
        if (eventItem.notes != null)
            eventItem.notes.text = notes;
        eventItem.startDate.text = baseEvent.startDate.text;
        eventItem.dueDate.text = baseEvent.dueDate.text;
        eventItem.hasStartDate = baseEvent.hasStartDate;
        eventItem.hasDueDate = baseEvent.hasDueDate;
        eventItem.isFavorite = baseEvent.isFavorite;

        eventItem.DisplayOptional();
        eventItem.ChangeFavorite();

        // places event on the full profile
        eventItem.transform.SetParent(AppManager.Instance.fullProfile.eventContainer, false);
        // Removes the initial event.
        allEvents.Remove(baseEvent);
        Destroy(baseEvent.gameObject);
        // adds the new event to the list and then sorts it
        allEvents.Add(eventItem);
        SortByCurrentCriteria();
        eventItem.SaveEvent();
    }

    // hides or shows all events
    public void ShowAllEvents(bool show)
    {
        foreach (EventItem item in allEvents)
        {
            item.gameObject.SetActive(show);
        }
    }

    // used for deleting the gameobjects when profile is deleted
    public void DeleteEventObjects()
    {
        foreach (EventItem item in allEvents)
        {
            Destroy(item.gameObject);
            
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
    public bool isArchived;
    public string archivedAge;
        
}