using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Unity.Notifications.Android;

public class EventItem : MonoBehaviour
{
    public CanvasGroupToggle deleteButtonCGT;
    public CanvasGroupToggle expandedContainer;
    public CanvasGroupToggle shortBackground;
    // corresponds to the profile id
    public int profileId;
    // to be loaded in order
    public int eventId;
    public TMP_Text title;
    public TMP_Text notes;
    public TMP_Text startDate, dueDate;

    public CanvasGroupToggle startDateContainer;
    public CanvasGroupToggle dueDateContainer;

    public bool isFavorite = false;
    public Image favoriteStar;

    public bool hasStartDate;
    public bool hasDueDate;

    // public bool getNotificationOnDue;

    // displays optional items
    // bool asks if full view is on to show expanded note view
    public void DisplayOptional(bool fullView)
    {
        startDateContainer.ShowElement(hasStartDate);
        dueDateContainer.ShowElement(hasDueDate);

        if (fullView)
        {
            if (notes.text != string.Empty)
            {
                ShowExpanded(true);
            }
            else
                ShowExpanded(false);
        }
    }

    public void ShowExpanded(bool _showExpanded)
    {
        RectTransform containerRect = transform.GetComponent<RectTransform>();

        if (_showExpanded)
        {
            expandedContainer.ShowElement();
            shortBackground.HideElement();
            // DOES NOT WORK 
            // containerRect.sizeDelta = new Vector2(0, 250);
        }
        else
        {
            expandedContainer.HideElement();
            shortBackground.ShowElement();
            // containerRect.sizeDelta = new Vector2(0, 100);
        }
    }

    public void ToggleFavorite()
    {
        isFavorite = !isFavorite;
        SaveEvent();
        ChangeFavorite();
    }

    public void ChangeFavorite()
    {
        if (isFavorite)
        {
            favoriteStar.sprite = AppManager.Instance.starFilled;
        }
        else
        {
            favoriteStar.sprite = AppManager.Instance.starHollow;
        }
    }

    public void SaveEvent()
    {
        AppManager.Instance.eventEdited = true;
        EventData data = new EventData();
    
        data.profileId = profileId;
        data.eventId = eventId;
        data.title = title.text;
        data.notes = notes.text;
        data.startDate = startDate.text;
        data.dueDate = dueDate.text;
        data.hasStartDate = hasStartDate;
        data.hasDueDate = hasDueDate;
        data.isFavorite = isFavorite;

        string json = JsonUtility.ToJson(data);

        string directoryPath = $"{Application.persistentDataPath}/{profileId}";
        if (!Directory.Exists(directoryPath))
        {
            // Create the directory
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = $"{directoryPath}/{eventId}.json";

        File.WriteAllText(filePath, json);
    }

    // deletes an event. Doesn't need confirmation. Located on onclick of eventitem delete button
    public void DeleteEvent()
    {
        // deletes self from saved events
        string path = $"{Application.persistentDataPath}/{profileId}/{eventId}.json";
        AppManager.Instance.DeleteItemAtPath(path);

        //TODO: delete notifications too


        Profile profile = AppManager.Instance.FindProfile(profileId);
        // removes self from profile's list of events
        profile.allEvents.Remove(this);

        AppManager.Instance.FadeAndDestroy(gameObject);
        //reorganize the short events list
        AppManager.Instance.eventEdited = true;
    }

    void HandleNotifications()
    {
        string newID = profileId.ToString() + eventId.ToString();
        var notificationID = int.Parse(newID);
        // AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", notificationId);
    }

    // brings up the event modal. Attached to edit button on eventitems prefabs
    public void StartEditEvent()
    {
        //if can't edit exits
        if (!AppManager.Instance.canEdit) return;

        // opens modal and sets event info
        NewEventHandler handler = AppManager.Instance.eventModal;
        handler.SetEditEvent(this);
        handler.cgtToHideOnSubmit.ShowElement();
    }

}

[System.Serializable]
public class EventData
{
    // corresponds to the profile id
    public int profileId;
    public int eventId;
    public string title;
    public string notes;
    public string startDate;
    public string dueDate;
    public bool hasStartDate; 
    public bool hasDueDate;
    public bool isFavorite;
}

public enum DataType
{
    ProfileID, EventID, Title, Notes, StartDate, DueDate, HasStart, HasDue, IsFavorite, All, Edit
}

